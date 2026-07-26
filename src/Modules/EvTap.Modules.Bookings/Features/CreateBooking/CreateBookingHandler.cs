using EvTap.Modules.Bookings.Domain;
using EvTap.Modules.Bookings.Persistence;
using EvTap.Shared.Contracts;
using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Bookings.Features.CreateBooking;

/// <summary>
/// Creates a pending booking, priced from whichever of the listing owner's PricePerHour /
/// PricePerNight matches the requested rental type — never derived or guessed. The renter pays
/// in a separate step (see Features/ProcessPayment) via the mock payment gateway.
/// </summary>
internal sealed class CreateBookingHandler(BookingsDbContext dbContext, ISender sender)
    : IRequestHandler<CreateBookingCommand, Result<CreateBookingResponse>>
{
    public async Task<Result<CreateBookingResponse>> Handle(
        CreateBookingCommand request,
        CancellationToken cancellationToken)
    {
        var rentalType = Enum.Parse<RentalType>(request.RentalType, ignoreCase: true);

        var listing = await sender.Send(new GetListingForBookingQuery(request.ListingId), cancellationToken);

        if (listing is null)
        {
            return Result.Failure<CreateBookingResponse>(
                Error.NotFound("Bookings.ListingNotFound", "Listing not found."));
        }

        if (listing.Status != "Active")
        {
            return Result.Failure<CreateBookingResponse>(
                Error.Conflict("Bookings.ListingNotBookable", "Only active listings can be booked."));
        }

        if (listing.OwnerId == request.RenterId)
        {
            return Result.Failure<CreateBookingResponse>(
                Error.Conflict("Bookings.CannotBookOwnListing", "You cannot book your own listing."));
        }

        var unitPriceAzn = rentalType == RentalType.Hourly ? listing.PricePerHour : listing.PricePerNight;

        if (unitPriceAzn is null)
        {
            var label = rentalType == RentalType.Hourly ? "saatlıq" : "gecəlik";
            return Result.Failure<CreateBookingResponse>(
                Error.Conflict(
                    "Bookings.RentalTypeNotAvailable",
                    $"Bu elan üçün {label} kirayə qiyməti təyin olunmayıb."));
        }

        var endAt = rentalType == RentalType.Hourly
            ? request.StartAt.AddHours(request.Units)
            : request.StartAt.AddDays(request.Units);

        var totalAzn = Math.Round(unitPriceAzn.Value * request.Units, 2, MidpointRounding.AwayFromZero);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            ListingId = listing.Id,
            ListingTitle = listing.Title,
            RenterId = request.RenterId,
            RenterEmail = request.RenterEmail,
            OwnerId = listing.OwnerId,
            RentalType = rentalType,
            StartAt = request.StartAt,
            EndAt = endAt,
            Units = request.Units,
            UnitPriceAzn = unitPriceAzn.Value,
            TotalPriceAzn = totalAzn,
            Status = BookingStatus.PendingPayment,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        dbContext.Bookings.Add(booking);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateBookingResponse(
            booking.Id,
            booking.RentalType.ToString(),
            booking.StartAt,
            booking.EndAt,
            booking.Units,
            booking.UnitPriceAzn,
            totalAzn));
    }
}

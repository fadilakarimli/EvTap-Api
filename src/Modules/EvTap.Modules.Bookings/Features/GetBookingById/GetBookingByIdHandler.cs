using EvTap.Modules.Bookings.Persistence;
using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Bookings.Features.GetBookingById;

internal sealed class GetBookingByIdHandler(BookingsDbContext dbContext)
    : IRequestHandler<GetBookingByIdQuery, Result<BookingDetailResponse>>
{
    public async Task<Result<BookingDetailResponse>> Handle(
        GetBookingByIdQuery request,
        CancellationToken cancellationToken)
    {
        var booking = await dbContext.Bookings.FindAsync([request.Id], cancellationToken);

        if (booking is null)
        {
            return Result.Failure<BookingDetailResponse>(
                Error.NotFound("Bookings.NotFound", "Booking not found."));
        }

        if (booking.RenterId != request.RequestingUserId)
        {
            return Result.Failure<BookingDetailResponse>(
                Error.Forbidden("Bookings.NotOwner", "This booking does not belong to you."));
        }

        return Result.Success(new BookingDetailResponse(
            booking.Id,
            booking.ListingId,
            booking.ListingTitle,
            booking.RentalType.ToString(),
            booking.StartAt,
            booking.EndAt,
            booking.Units,
            booking.UnitPriceAzn,
            booking.TotalPriceAzn,
            booking.Status.ToString(),
            booking.PaymentReference,
            booking.CardLast4,
            booking.CreatedAt,
            booking.PaidAt));
    }
}

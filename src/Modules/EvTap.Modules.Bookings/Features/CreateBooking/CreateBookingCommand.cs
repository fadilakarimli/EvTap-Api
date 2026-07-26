using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Bookings.Features.CreateBooking;

public sealed record CreateBookingCommand(
    Guid ListingId,
    Guid RenterId,
    string RenterEmail,
    string RentalType,
    DateTimeOffset StartAt,
    int Units) : IRequest<Result<CreateBookingResponse>>;

public sealed record CreateBookingResponse(
    Guid BookingId,
    string RentalType,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    int Units,
    decimal UnitPriceAzn,
    decimal TotalPriceAzn);

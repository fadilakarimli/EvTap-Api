using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Bookings.Features.GetBookingById;

public sealed record GetBookingByIdQuery(Guid Id, Guid RequestingUserId) : IRequest<Result<BookingDetailResponse>>;

public sealed record BookingDetailResponse(
    Guid Id,
    Guid ListingId,
    string ListingTitle,
    string RentalType,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    int Units,
    decimal UnitPriceAzn,
    decimal TotalPriceAzn,
    string Status,
    string? PaymentReference,
    string? CardLast4,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt);

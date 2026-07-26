using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Bookings.Features.GetMyBookings;

public sealed record GetMyBookingsQuery(Guid RenterId) : IRequest<Result<IReadOnlyList<BookingSummaryResponse>>>;

public sealed record BookingSummaryResponse(
    Guid Id,
    Guid ListingId,
    string ListingTitle,
    string RentalType,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    int Units,
    decimal TotalPriceAzn,
    string Status,
    DateTimeOffset CreatedAt);

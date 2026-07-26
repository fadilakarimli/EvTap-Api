using EvTap.Modules.Bookings.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Bookings.Features.GetMyBookings;

internal sealed class GetMyBookingsHandler(BookingsDbContext dbContext)
    : IRequestHandler<GetMyBookingsQuery, Result<IReadOnlyList<BookingSummaryResponse>>>
{
    public async Task<Result<IReadOnlyList<BookingSummaryResponse>>> Handle(
        GetMyBookingsQuery request,
        CancellationToken cancellationToken)
    {
        var bookings = await dbContext.Bookings
            .Where(b => b.RenterId == request.RenterId)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new BookingSummaryResponse(
                b.Id,
                b.ListingId,
                b.ListingTitle,
                b.RentalType.ToString(),
                b.StartAt,
                b.EndAt,
                b.Units,
                b.TotalPriceAzn,
                b.Status.ToString(),
                b.CreatedAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<BookingSummaryResponse>>(bookings);
    }
}

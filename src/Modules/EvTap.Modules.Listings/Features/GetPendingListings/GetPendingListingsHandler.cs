using EvTap.Modules.Listings.Domain;
using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Listings.Features.GetPendingListings;

internal sealed class GetPendingListingsHandler(ListingsDbContext dbContext)
    : IRequestHandler<GetPendingListingsQuery, Result<IReadOnlyList<PendingListingResponse>>>
{
    public async Task<Result<IReadOnlyList<PendingListingResponse>>> Handle(
        GetPendingListingsQuery request,
        CancellationToken cancellationToken)
    {
        var listings = await dbContext.Listings
            .Where(l => l.Status == ListingStatus.Pending)
            .OrderBy(l => l.CreatedAt)
            .Select(l => new PendingListingResponse(
                l.Id,
                l.Title,
                l.Price,
                l.Rooms,
                l.District,
                l.OwnerId,
                l.CreatedAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<PendingListingResponse>>(listings);
    }
}

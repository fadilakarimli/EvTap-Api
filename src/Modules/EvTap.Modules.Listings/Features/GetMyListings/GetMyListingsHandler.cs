using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Listings.Features.GetMyListings;

internal sealed class GetMyListingsHandler(ListingsDbContext dbContext)
    : IRequestHandler<GetMyListingsQuery, Result<IReadOnlyList<MyListingResponse>>>
{
    public async Task<Result<IReadOnlyList<MyListingResponse>>> Handle(
        GetMyListingsQuery request,
        CancellationToken cancellationToken)
    {
        var listings = await dbContext.Listings
            .Where(l => l.OwnerId == request.OwnerId)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new MyListingResponse(
                l.Id,
                l.Title,
                l.Price,
                l.Rooms,
                l.District,
                l.Status.ToString(),
                l.ViewCount,
                l.CreatedAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<MyListingResponse>>(listings);
    }
}

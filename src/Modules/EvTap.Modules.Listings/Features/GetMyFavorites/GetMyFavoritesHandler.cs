using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Listings.Features.GetMyFavorites;

internal sealed class GetMyFavoritesHandler(ListingsDbContext dbContext)
    : IRequestHandler<GetMyFavoritesQuery, Result<IReadOnlyList<FavoriteListingResponse>>>
{
    public async Task<Result<IReadOnlyList<FavoriteListingResponse>>> Handle(
        GetMyFavoritesQuery request,
        CancellationToken cancellationToken)
    {
        var favorites = await dbContext.Favorites
            .Where(f => f.UserId == request.UserId)
            .OrderByDescending(f => f.CreatedAt)
            .Join(
                dbContext.Listings,
                f => f.ListingId,
                l => l.Id,
                (f, l) => new FavoriteListingResponse(
                    l.Id,
                    l.Title,
                    l.Price,
                    l.Rooms,
                    l.District,
                    l.Status.ToString(),
                    l.ViewCount,
                    l.CreatedAt,
                    l.Images.OrderBy(i => i.Order).Select(i => i.Url).FirstOrDefault()))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<FavoriteListingResponse>>(favorites);
    }
}

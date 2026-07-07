using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Listings.Features.RemoveFavorite;

internal sealed class RemoveFavoriteHandler(ListingsDbContext dbContext)
    : IRequestHandler<RemoveFavoriteCommand, Result>
{
    public async Task<Result> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
    {
        var favorite = await dbContext.Favorites
            .SingleOrDefaultAsync(
                f => f.UserId == request.UserId && f.ListingId == request.ListingId,
                cancellationToken);

        if (favorite is null)
        {
            return Result.Failure(Error.NotFound("Listings.FavoriteNotFound", "Favorite not found."));
        }

        dbContext.Favorites.Remove(favorite);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

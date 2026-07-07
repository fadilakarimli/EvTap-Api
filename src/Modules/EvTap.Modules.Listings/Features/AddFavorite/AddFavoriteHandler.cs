using EvTap.Modules.Listings.Domain;
using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Listings.Features.AddFavorite;

internal sealed class AddFavoriteHandler(ListingsDbContext dbContext)
    : IRequestHandler<AddFavoriteCommand, Result>
{
    public async Task<Result> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
    {
        var listingExists = await dbContext.Listings
            .AnyAsync(l => l.Id == request.ListingId, cancellationToken);

        if (!listingExists)
        {
            return Result.Failure(Error.NotFound("Listings.NotFound", "Listing not found."));
        }

        var alreadyFavorited = await dbContext.Favorites
            .AnyAsync(f => f.UserId == request.UserId && f.ListingId == request.ListingId, cancellationToken);

        if (alreadyFavorited)
        {
            return Result.Success();
        }

        dbContext.Favorites.Add(new Favorite
        {
            UserId = request.UserId,
            ListingId = request.ListingId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

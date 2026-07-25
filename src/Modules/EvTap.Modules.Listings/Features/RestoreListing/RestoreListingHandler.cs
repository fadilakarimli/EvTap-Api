using EvTap.Modules.Listings.Domain;
using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.RestoreListing;

internal sealed class RestoreListingHandler(ListingsDbContext dbContext)
    : IRequestHandler<RestoreListingCommand, Result>
{
    public async Task<Result> Handle(RestoreListingCommand request, CancellationToken cancellationToken)
    {
        var listing = await dbContext.Listings.FindAsync([request.Id], cancellationToken);

        if (listing is null)
        {
            return Result.Failure(Error.NotFound("Listings.NotFound", "Listing not found."));
        }

        if (listing.Status != ListingStatus.Removed)
        {
            return Result.Failure(Error.Conflict(
                "Listings.NotArchived",
                "Only archived (removed) listings can be restored."));
        }

        // Restored listings go back to Active directly — an admin archiving/restoring their
        // own decision doesn't need to be re-approved from scratch.
        listing.Status = ListingStatus.Active;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

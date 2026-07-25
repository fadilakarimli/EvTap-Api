using EvTap.Modules.Listings.Domain;
using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.DeleteListing;

internal sealed class DeleteListingHandler(ListingsDbContext dbContext)
    : IRequestHandler<DeleteListingCommand, Result>
{
    public async Task<Result> Handle(DeleteListingCommand request, CancellationToken cancellationToken)
    {
        var listing = await dbContext.Listings.FindAsync([request.Id], cancellationToken);

        if (listing is null)
        {
            return Result.Failure(Error.NotFound("Listings.NotFound", "Listing not found."));
        }

        if (listing.OwnerId != request.RequestingUserId && !request.IsAdmin)
        {
            return Result.Failure(Error.Forbidden("Listings.NotOwner", "You do not own this listing."));
        }

        listing.Status = ListingStatus.Removed;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

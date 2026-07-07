using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.UpdateListing;

internal sealed class UpdateListingHandler(ListingsDbContext dbContext)
    : IRequestHandler<UpdateListingCommand, Result>
{
    public async Task<Result> Handle(UpdateListingCommand request, CancellationToken cancellationToken)
    {
        var listing = await dbContext.Listings.FindAsync([request.Id], cancellationToken);

        if (listing is null)
        {
            return Result.Failure(Error.NotFound("Listings.NotFound", "Listing not found."));
        }

        if (listing.OwnerId != request.RequestingUserId)
        {
            return Result.Failure(Error.Forbidden("Listings.NotOwner", "You do not own this listing."));
        }

        listing.Title = request.Title;
        listing.Description = request.Description;
        listing.Price = request.Price;
        listing.Rooms = request.Rooms;
        listing.AreaSquareMeters = request.AreaSquareMeters;
        listing.District = request.District;
        listing.Address = request.Address;
        listing.Latitude = request.Latitude;
        listing.Longitude = request.Longitude;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

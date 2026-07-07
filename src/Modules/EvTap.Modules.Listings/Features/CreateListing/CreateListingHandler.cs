using EvTap.Modules.Listings.Domain;
using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.CreateListing;

internal sealed class CreateListingHandler(ListingsDbContext dbContext)
    : IRequestHandler<CreateListingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateListingCommand request, CancellationToken cancellationToken)
    {
        var listing = new Listing
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Rooms = request.Rooms,
            AreaSquareMeters = request.AreaSquareMeters,
            District = request.District,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Status = ListingStatus.Pending,
            OwnerId = request.OwnerId,
            CreatedAt = DateTimeOffset.UtcNow,
            ViewCount = 0,
        };

        dbContext.Listings.Add(listing);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(listing.Id);
    }
}

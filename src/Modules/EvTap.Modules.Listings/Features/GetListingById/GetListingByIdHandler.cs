using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Listings.Features.GetListingById;

internal sealed class GetListingByIdHandler(ListingsDbContext dbContext)
    : IRequestHandler<GetListingByIdQuery, Result<ListingDetailResponse>>
{
    public async Task<Result<ListingDetailResponse>> Handle(
        GetListingByIdQuery request,
        CancellationToken cancellationToken)
    {
        var listing = await dbContext.Listings
            .Include(l => l.Images)
            .SingleOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (listing is null)
        {
            return Result.Failure<ListingDetailResponse>(
                Error.NotFound("Listings.NotFound", "Listing not found."));
        }

        listing.ViewCount++;
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new ListingDetailResponse(
            listing.Id,
            listing.Title,
            listing.Description,
            listing.Price,
            listing.Rooms,
            listing.AreaSquareMeters,
            listing.District,
            listing.Address,
            listing.Latitude,
            listing.Longitude,
            listing.Status.ToString(),
            listing.OwnerId,
            listing.CreatedAt,
            listing.ViewCount,
            listing.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList());

        return Result.Success(response);
    }
}

using EvTap.Modules.Listings.Domain;
using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Listings.Features.GetAllListings;

internal sealed class GetAllListingsHandler(ListingsDbContext dbContext)
    : IRequestHandler<GetAllListingsQuery, Result<IReadOnlyList<AdminListingResponse>>>
{
    public async Task<Result<IReadOnlyList<AdminListingResponse>>> Handle(
        GetAllListingsQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Listings.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<ListingStatus>(request.Status, ignoreCase: true, out var status))
            {
                return Result.Failure<IReadOnlyList<AdminListingResponse>>(
                    Error.Validation("Listings.InvalidStatus", $"Unknown status '{request.Status}'."));
            }

            query = query.Where(l => l.Status == status);
        }

        var listings = await query
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new AdminListingResponse(
                l.Id,
                l.Title,
                l.Price,
                l.Rooms,
                l.District,
                l.Status.ToString(),
                l.OwnerId,
                l.ViewCount,
                l.CreatedAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<AdminListingResponse>>(listings);
    }
}

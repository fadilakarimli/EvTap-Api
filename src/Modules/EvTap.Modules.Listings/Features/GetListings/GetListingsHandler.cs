using EvTap.Modules.Listings.Domain;
using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace EvTap.Modules.Listings.Features.GetListings;

internal sealed class GetListingsHandler(ListingsDbContext dbContext, HybridCache cache)
    : IRequestHandler<GetListingsQuery, Result<PagedResult<ListingSummaryResponse>>>
{
    private static readonly HybridCacheEntryOptions CacheOptions = new()
    {
        Expiration = TimeSpan.FromSeconds(60),
        LocalCacheExpiration = TimeSpan.FromSeconds(60),
    };

    public async Task<Result<PagedResult<ListingSummaryResponse>>> Handle(
        GetListingsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = BuildCacheKey(request);

        var result = await cache.GetOrCreateAsync(
            cacheKey,
            async ct => await QueryListingsAsync(request, ct),
            CacheOptions,
            cancellationToken: cancellationToken);

        return Result.Success(result);
    }

    private async Task<PagedResult<ListingSummaryResponse>> QueryListingsAsync(
        GetListingsQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Listings
            .Where(l => l.Status == ListingStatus.Active)
            .AsNoTracking();

        if (request.MinPrice.HasValue)
        {
            query = query.Where(l => l.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(l => l.Price <= request.MaxPrice.Value);
        }

        if (request.Rooms.HasValue)
        {
            query = query.Where(l => l.Rooms == request.Rooms.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.District))
        {
            query = query.Where(l => l.District == request.District);
        }

        query = request.SortBy switch
        {
            "price_asc" => query.OrderBy(l => l.Price),
            "price_desc" => query.OrderByDescending(l => l.Price),
            "oldest" => query.OrderBy(l => l.CreatedAt),
            _ => query.OrderByDescending(l => l.CreatedAt),
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new ListingSummaryResponse(
                l.Id,
                l.Title,
                l.Price,
                l.Rooms,
                l.AreaSquareMeters,
                l.District,
                l.Address,
                l.ViewCount,
                l.CreatedAt,
                l.Images.OrderBy(i => i.Order).Select(i => i.Url).FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return new PagedResult<ListingSummaryResponse>(items, request.Page, request.PageSize, totalCount);
    }

    private static string BuildCacheKey(GetListingsQuery request) =>
        $"listings:{request.MinPrice}:{request.MaxPrice}:{request.Rooms}:{request.District}:{request.SortBy}:{request.Page}:{request.PageSize}";
}

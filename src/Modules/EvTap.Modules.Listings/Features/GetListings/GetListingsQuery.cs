using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.GetListings;

public sealed record GetListingsQuery(
    decimal? MinPrice,
    decimal? MaxPrice,
    int? Rooms,
    string? District,
    string? SortBy,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<ListingSummaryResponse>>>;

public sealed record ListingSummaryResponse(
    Guid Id,
    string Title,
    decimal Price,
    int Rooms,
    double AreaSquareMeters,
    string District,
    string Address,
    int ViewCount,
    DateTimeOffset CreatedAt,
    string? ThumbnailUrl);

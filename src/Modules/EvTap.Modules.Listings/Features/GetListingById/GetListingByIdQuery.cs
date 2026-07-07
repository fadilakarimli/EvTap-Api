using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.GetListingById;

public sealed record GetListingByIdQuery(Guid Id) : IRequest<Result<ListingDetailResponse>>;

public sealed record ListingDetailResponse(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    int Rooms,
    double AreaSquareMeters,
    string District,
    string Address,
    double Latitude,
    double Longitude,
    string Status,
    Guid OwnerId,
    DateTimeOffset CreatedAt,
    int ViewCount,
    IReadOnlyList<string> ImageUrls);

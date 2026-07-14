using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.GetMyListings;

public sealed record GetMyListingsQuery(Guid OwnerId) : IRequest<Result<IReadOnlyList<MyListingResponse>>>;

public sealed record MyListingResponse(
    Guid Id,
    string Title,
    decimal Price,
    int Rooms,
    string District,
    string Status,
    int ViewCount,
    DateTimeOffset CreatedAt);

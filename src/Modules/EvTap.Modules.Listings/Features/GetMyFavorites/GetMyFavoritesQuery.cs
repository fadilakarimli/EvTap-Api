using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.GetMyFavorites;

public sealed record GetMyFavoritesQuery(Guid UserId) : IRequest<Result<IReadOnlyList<FavoriteListingResponse>>>;

public sealed record FavoriteListingResponse(
    Guid Id,
    string Title,
    decimal Price,
    int Rooms,
    string District,
    string Status,
    int ViewCount,
    DateTimeOffset CreatedAt,
    string? ThumbnailUrl);

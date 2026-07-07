using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.RemoveFavorite;

public sealed record RemoveFavoriteCommand(Guid ListingId, Guid UserId) : IRequest<Result>;

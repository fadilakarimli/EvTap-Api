using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.AddFavorite;

public sealed record AddFavoriteCommand(Guid ListingId, Guid UserId) : IRequest<Result>;

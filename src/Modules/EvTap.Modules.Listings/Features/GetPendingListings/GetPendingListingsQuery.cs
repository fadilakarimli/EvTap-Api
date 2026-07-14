using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.GetPendingListings;

public sealed record GetPendingListingsQuery : IRequest<Result<IReadOnlyList<PendingListingResponse>>>;

public sealed record PendingListingResponse(
    Guid Id,
    string Title,
    decimal Price,
    int Rooms,
    string District,
    Guid OwnerId,
    DateTimeOffset CreatedAt);

using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Listings.Features.GetAllListings;

/// <summary>Admin-only: every listing regardless of status, optionally filtered by status.</summary>
public sealed record GetAllListingsQuery(string? Status) : IRequest<Result<IReadOnlyList<AdminListingResponse>>>;

public sealed record AdminListingResponse(
    Guid Id,
    string Title,
    decimal Price,
    int Rooms,
    string District,
    string Status,
    Guid OwnerId,
    int ViewCount,
    DateTimeOffset CreatedAt);

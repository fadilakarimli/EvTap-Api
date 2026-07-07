using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.SavedSearches.Features.GetMySavedSearches;

public sealed record GetMySavedSearchesQuery(Guid UserId) : IRequest<Result<IReadOnlyList<SavedSearchResponse>>>;

public sealed record SavedSearchResponse(
    Guid Id,
    decimal? MinPrice,
    decimal? MaxPrice,
    int? MinRooms,
    string? District,
    bool IsActive,
    DateTimeOffset CreatedAt);

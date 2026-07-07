using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.SavedSearches.Features.CreateSavedSearch;

public sealed record CreateSavedSearchCommand(
    Guid UserId,
    decimal? MinPrice,
    decimal? MaxPrice,
    int? MinRooms,
    string? District) : IRequest<Result<Guid>>;

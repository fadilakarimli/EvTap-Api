using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.SavedSearches.Features.DeleteSavedSearch;

public sealed record DeleteSavedSearchCommand(Guid Id, Guid RequestingUserId) : IRequest<Result>;

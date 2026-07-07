using EvTap.Modules.SavedSearches.Persistence;
using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.SavedSearches.Features.DeleteSavedSearch;

internal sealed class DeleteSavedSearchHandler(SavedSearchesDbContext dbContext)
    : IRequestHandler<DeleteSavedSearchCommand, Result>
{
    public async Task<Result> Handle(DeleteSavedSearchCommand request, CancellationToken cancellationToken)
    {
        var savedSearch = await dbContext.SavedSearches.FindAsync([request.Id], cancellationToken);

        if (savedSearch is null)
        {
            return Result.Failure(Error.NotFound("SavedSearches.NotFound", "Saved search not found."));
        }

        if (savedSearch.UserId != request.RequestingUserId)
        {
            return Result.Failure(Error.Forbidden("SavedSearches.NotOwner", "You do not own this saved search."));
        }

        dbContext.SavedSearches.Remove(savedSearch);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

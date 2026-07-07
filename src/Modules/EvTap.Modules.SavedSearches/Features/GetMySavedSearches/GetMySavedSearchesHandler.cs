using EvTap.Modules.SavedSearches.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.SavedSearches.Features.GetMySavedSearches;

internal sealed class GetMySavedSearchesHandler(SavedSearchesDbContext dbContext)
    : IRequestHandler<GetMySavedSearchesQuery, Result<IReadOnlyList<SavedSearchResponse>>>
{
    public async Task<Result<IReadOnlyList<SavedSearchResponse>>> Handle(
        GetMySavedSearchesQuery request,
        CancellationToken cancellationToken)
    {
        var searches = await dbContext.SavedSearches
            .Where(s => s.UserId == request.UserId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SavedSearchResponse(
                s.Id,
                s.MinPrice,
                s.MaxPrice,
                s.MinRooms,
                s.District,
                s.IsActive,
                s.CreatedAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<SavedSearchResponse>>(searches);
    }
}

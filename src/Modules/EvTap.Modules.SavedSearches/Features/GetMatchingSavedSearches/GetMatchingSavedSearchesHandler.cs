using EvTap.Modules.SavedSearches.Persistence;
using EvTap.Shared.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.SavedSearches.Features.GetMatchingSavedSearches;

/// <summary>
/// Handles the cross-module read query defined in EvTap.Shared.Contracts. Consumed in-process
/// by the Notifications module's ListingApprovedConsumer (documented trade-off: an in-process
/// MediatR query against this module's read model instead of a local read-model copy — the
/// simpler choice inside a modular monolith).
/// </summary>
internal sealed class GetMatchingSavedSearchesHandler(SavedSearchesDbContext dbContext)
    : IRequestHandler<GetMatchingSavedSearchesQuery, IReadOnlyList<MatchingSavedSearch>>
{
    public async Task<IReadOnlyList<MatchingSavedSearch>> Handle(
        GetMatchingSavedSearchesQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.SavedSearches
            .Where(s => s.IsActive)
            .Where(s => s.MinPrice == null || request.Price >= s.MinPrice)
            .Where(s => s.MaxPrice == null || request.Price <= s.MaxPrice)
            .Where(s => s.MinRooms == null || request.Rooms >= s.MinRooms)
            .Where(s => s.District == null || s.District == request.District)
            .Select(s => new MatchingSavedSearch(s.Id, s.UserId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

using EvTap.Modules.Users.Persistence;
using EvTap.Shared.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Users.Features.GetUserEmails;

/// <summary>
/// Handles the cross-module read query defined in EvTap.Shared.Contracts. Consumed in-process
/// by the Notifications module's ListingApprovedConsumer to resolve recipient email addresses
/// (same documented trade-off as the SavedSearches matching query).
/// </summary>
internal sealed class GetUserEmailsHandler(UsersDbContext dbContext)
    : IRequestHandler<GetUserEmailsQuery, IReadOnlyDictionary<Guid, string>>
{
    public async Task<IReadOnlyDictionary<Guid, string>> Handle(
        GetUserEmailsQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .Where(u => request.UserIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Email })
            .AsNoTracking()
            .ToDictionaryAsync(u => u.Id, u => u.Email, cancellationToken);
    }
}

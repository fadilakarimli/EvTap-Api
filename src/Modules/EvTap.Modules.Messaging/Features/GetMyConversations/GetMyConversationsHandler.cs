using EvTap.Modules.Messaging.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Messaging.Features.GetMyConversations;

internal sealed class GetMyConversationsHandler(MessagingDbContext dbContext)
    : IRequestHandler<GetMyConversationsQuery, Result<IReadOnlyList<ConversationSummary>>>
{
    public async Task<Result<IReadOnlyList<ConversationSummary>>> Handle(
        GetMyConversationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = request.CurrentUserId;

        var messages = await dbContext.Messages
            .Where(m => m.SenderId == userId || m.RecipientId == userId)
            .OrderByDescending(m => m.SentAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Group in memory by (listing, other participant); first item per group is the latest
        // message because of the ordering above.
        var conversations = messages
            .GroupBy(m => (m.ListingId, OtherId: m.SenderId == userId ? m.RecipientId : m.SenderId))
            .Select(group =>
            {
                var last = group.First();
                var otherName = group
                    .Where(m => m.SenderId == group.Key.OtherId)
                    .Select(m => m.SenderName)
                    .FirstOrDefault() ?? "İstifadəçi";

                return new ConversationSummary(
                    group.Key.ListingId,
                    last.ListingTitle,
                    group.Key.OtherId,
                    otherName,
                    last.Text,
                    last.SentAt);
            })
            .OrderByDescending(c => c.LastMessageAt)
            .ToList();

        return Result.Success<IReadOnlyList<ConversationSummary>>(conversations);
    }
}

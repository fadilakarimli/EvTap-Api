using EvTap.Modules.Messaging.Features.SendMessage;
using EvTap.Modules.Messaging.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Messaging.Features.GetConversation;

internal sealed class GetConversationHandler(MessagingDbContext dbContext)
    : IRequestHandler<GetConversationQuery, Result<IReadOnlyList<MessageResponse>>>
{
    public async Task<Result<IReadOnlyList<MessageResponse>>> Handle(
        GetConversationQuery request,
        CancellationToken cancellationToken)
    {
        var messages = await dbContext.Messages
            .Where(m => m.ListingId == request.ListingId)
            .Where(m =>
                (m.SenderId == request.CurrentUserId && m.RecipientId == request.OtherUserId) ||
                (m.SenderId == request.OtherUserId && m.RecipientId == request.CurrentUserId))
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageResponse(
                m.Id,
                m.ListingId,
                m.ListingTitle,
                m.SenderId,
                m.SenderName,
                m.RecipientId,
                m.Text,
                m.SentAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<MessageResponse>>(messages);
    }
}

using EvTap.Modules.Notifications.Realtime;
using EvTap.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace EvTap.Modules.Notifications.Consumers;

/// <summary>Pushes a real-time SignalR event to the message recipient. Deliberately no
/// Notification row / email here — chat messages are a lighter-weight, higher-frequency signal
/// than a matched-listing alert, and the recipient can always see full history in Messages.</summary>
public sealed class MessageSentConsumer(IHubContext<NotificationsHub> hubContext) : IConsumer<MessageSentEvent>
{
    public Task Consume(ConsumeContext<MessageSentEvent> context)
    {
        var message = context.Message;

        return hubContext.Clients.User(message.RecipientId.ToString()).SendAsync(
            "NewMessage",
            new
            {
                message.ListingId,
                message.ListingTitle,
                message.SenderId,
                message.SenderName,
                message.Text,
                message.SentAt,
            },
            context.CancellationToken);
    }
}

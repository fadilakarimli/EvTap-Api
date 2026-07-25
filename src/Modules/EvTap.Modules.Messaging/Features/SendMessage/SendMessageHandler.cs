using EvTap.Modules.Messaging.Domain;
using EvTap.Modules.Messaging.Persistence;
using EvTap.Shared.Contracts;
using EvTap.Shared.Results;
using MassTransit;
using MediatR;

namespace EvTap.Modules.Messaging.Features.SendMessage;

internal sealed class SendMessageHandler(MessagingDbContext dbContext, IPublishEndpoint publishEndpoint)
    : IRequestHandler<SendMessageCommand, Result<MessageResponse>>
{
    public async Task<Result<MessageResponse>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ListingId = request.ListingId,
            ListingTitle = request.ListingTitle,
            SenderId = request.SenderId,
            SenderName = request.SenderName,
            RecipientId = request.RecipientId,
            Text = request.Text,
            SentAt = DateTimeOffset.UtcNow,
        };

        dbContext.Messages.Add(message);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Notifications module reacts to this (via RabbitMQ, no direct project reference) to
        // push a real-time SignalR event to the recipient.
        await publishEndpoint.Publish(
            new MessageSentEvent(
                message.Id,
                message.ListingId,
                message.ListingTitle,
                message.SenderId,
                message.SenderName,
                message.RecipientId,
                message.Text,
                message.SentAt),
            cancellationToken);

        return Result.Success(new MessageResponse(
            message.Id,
            message.ListingId,
            message.ListingTitle,
            message.SenderId,
            message.SenderName,
            message.RecipientId,
            message.Text,
            message.SentAt));
    }
}

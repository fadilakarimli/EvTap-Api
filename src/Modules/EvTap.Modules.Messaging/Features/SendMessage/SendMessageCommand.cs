using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Messaging.Features.SendMessage;

public sealed record SendMessageCommand(
    Guid ListingId,
    string ListingTitle,
    Guid SenderId,
    string SenderName,
    Guid RecipientId,
    string Text) : IRequest<Result<MessageResponse>>;

public sealed record MessageResponse(
    Guid Id,
    Guid ListingId,
    string ListingTitle,
    Guid SenderId,
    string SenderName,
    Guid RecipientId,
    string Text,
    DateTimeOffset SentAt);

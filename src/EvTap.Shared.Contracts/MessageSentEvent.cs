namespace EvTap.Shared.Contracts;

public sealed record MessageSentEvent(
    Guid MessageId,
    Guid ListingId,
    string ListingTitle,
    Guid SenderId,
    string SenderName,
    Guid RecipientId,
    string Text,
    DateTimeOffset SentAt);

namespace EvTap.Modules.Messaging.Domain;

public sealed class Message
{
    public Guid Id { get; init; }

    /// <summary>The listing this conversation is about.</summary>
    public Guid ListingId { get; init; }

    /// <summary>Denormalized at send time so conversation lists don't need a cross-module query.</summary>
    public required string ListingTitle { get; init; }

    public Guid SenderId { get; init; }

    /// <summary>Denormalized from the sender's JWT name claim at send time.</summary>
    public required string SenderName { get; init; }

    public Guid RecipientId { get; init; }

    public required string Text { get; set; }

    public DateTimeOffset SentAt { get; init; }
}

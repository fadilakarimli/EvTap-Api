namespace EvTap.Modules.Notifications.Domain;

public enum NotificationChannel
{
    Email,
    SignalR,
}

public sealed class Notification
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    /// <summary>Denormalized at write time (the consumer already resolves it) so the admin's
    /// system-wide notification view doesn't need a cross-module query per row.</summary>
    public required string RecipientEmail { get; init; }

    public Guid ListingId { get; init; }

    public required string ListingTitle { get; init; }

    public NotificationChannel Channel { get; init; }

    public DateTimeOffset SentAt { get; init; }
}

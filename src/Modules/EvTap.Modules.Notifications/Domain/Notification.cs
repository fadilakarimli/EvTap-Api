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

    public Guid ListingId { get; init; }

    public NotificationChannel Channel { get; init; }

    public DateTimeOffset SentAt { get; init; }
}

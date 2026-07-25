using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Notifications.Features.GetAllNotifications;

/// <summary>Admin-only: every notification sent across the whole system.</summary>
public sealed record GetAllNotificationsQuery : IRequest<Result<IReadOnlyList<AdminNotificationResponse>>>;

public sealed record AdminNotificationResponse(
    Guid Id,
    Guid UserId,
    string RecipientEmail,
    Guid ListingId,
    string ListingTitle,
    string Channel,
    DateTimeOffset SentAt);

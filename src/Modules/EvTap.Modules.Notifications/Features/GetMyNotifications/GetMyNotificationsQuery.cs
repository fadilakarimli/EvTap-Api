using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Notifications.Features.GetMyNotifications;

public sealed record GetMyNotificationsQuery(Guid UserId) : IRequest<Result<IReadOnlyList<NotificationResponse>>>;

public sealed record NotificationResponse(
    Guid Id,
    Guid ListingId,
    string Channel,
    DateTimeOffset SentAt);

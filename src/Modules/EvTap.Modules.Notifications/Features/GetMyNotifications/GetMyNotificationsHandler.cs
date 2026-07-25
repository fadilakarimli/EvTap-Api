using EvTap.Modules.Notifications.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Notifications.Features.GetMyNotifications;

internal sealed class GetMyNotificationsHandler(NotificationsDbContext dbContext)
    : IRequestHandler<GetMyNotificationsQuery, Result<IReadOnlyList<NotificationResponse>>>
{
    public async Task<Result<IReadOnlyList<NotificationResponse>>> Handle(
        GetMyNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await dbContext.Notifications
            .Where(n => n.UserId == request.UserId)
            .OrderByDescending(n => n.SentAt)
            .Select(n => new NotificationResponse(
                n.Id,
                n.ListingId,
                n.ListingTitle,
                n.Channel.ToString(),
                n.SentAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<NotificationResponse>>(notifications);
    }
}

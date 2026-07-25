using EvTap.Modules.Notifications.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Notifications.Features.GetAllNotifications;

internal sealed class GetAllNotificationsHandler(NotificationsDbContext dbContext)
    : IRequestHandler<GetAllNotificationsQuery, Result<IReadOnlyList<AdminNotificationResponse>>>
{
    public async Task<Result<IReadOnlyList<AdminNotificationResponse>>> Handle(
        GetAllNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await dbContext.Notifications
            .OrderByDescending(n => n.SentAt)
            .Select(n => new AdminNotificationResponse(
                n.Id,
                n.UserId,
                n.RecipientEmail,
                n.ListingId,
                n.ListingTitle,
                n.Channel.ToString(),
                n.SentAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<AdminNotificationResponse>>(notifications);
    }
}

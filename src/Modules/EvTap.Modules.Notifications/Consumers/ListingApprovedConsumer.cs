using EvTap.Modules.Notifications.Domain;
using EvTap.Modules.Notifications.Email;
using EvTap.Modules.Notifications.Persistence;
using EvTap.Modules.Notifications.Realtime;
using EvTap.Shared.Contracts;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EvTap.Modules.Notifications.Consumers;

/// <summary>
/// Reacts to a listing being approved: finds users whose active saved searches match the
/// listing, then notifies each of them by email (log-only in dev) and SignalR push, and
/// records Notification rows. Idempotent — a user is never notified twice for the same
/// listing (guarded both by a pre-check and a unique DB index on UserId+ListingId+Channel).
///
/// Saved-search matching and email resolution are done via in-process MediatR queries into
/// the SavedSearches/Users modules' read models (documented trade-off vs. a local read-model
/// copy — the simpler choice inside a modular monolith).
/// </summary>
public sealed class ListingApprovedConsumer(
    NotificationsDbContext dbContext,
    ISender sender,
    IEmailSender emailSender,
    IHubContext<NotificationsHub> hubContext,
    ILogger<ListingApprovedConsumer> logger) : IConsumer<ListingApprovedEvent>
{
    public async Task Consume(ConsumeContext<ListingApprovedEvent> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        var matches = await sender.Send(
            new GetMatchingSavedSearchesQuery(message.Price, message.Rooms, message.District),
            cancellationToken);

        var userIds = matches
            .Select(m => m.UserId)
            .Where(userId => userId != message.OwnerId)
            .Distinct()
            .ToList();

        if (userIds.Count == 0)
        {
            logger.LogInformation("No saved searches match listing {ListingId}", message.ListingId);
            return;
        }

        // Idempotency: skip users already notified about this listing (e.g. redelivery).
        var alreadyNotified = await dbContext.Notifications
            .Where(n => n.ListingId == message.ListingId && userIds.Contains(n.UserId))
            .Select(n => n.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var usersToNotify = userIds.Except(alreadyNotified).ToList();

        if (usersToNotify.Count == 0)
        {
            logger.LogInformation("All matching users already notified for listing {ListingId}", message.ListingId);
            return;
        }

        var emails = await sender.Send(new GetUserEmailsQuery(usersToNotify), cancellationToken);

        var subject = $"New listing matches your saved search: {message.Title}";
        var body =
            $"A new listing was just published on EvTap:\n\n" +
            $"{message.Title}\n" +
            $"District: {message.District}\n" +
            $"Rooms: {message.Rooms}\n" +
            $"Price: {message.Price:N0}\n\n" +
            $"Listing id: {message.ListingId}";

        var sentAt = DateTimeOffset.UtcNow;

        foreach (var userId in usersToNotify)
        {
            if (emails.TryGetValue(userId, out var email))
            {
                await emailSender.SendAsync(email, subject, body, cancellationToken);

                dbContext.Notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ListingId = message.ListingId,
                    Channel = NotificationChannel.Email,
                    SentAt = sentAt,
                });
            }

            await hubContext.Clients.User(userId.ToString()).SendAsync(
                "ListingMatched",
                new
                {
                    message.ListingId,
                    message.Title,
                    message.Price,
                    message.Rooms,
                    message.District,
                },
                cancellationToken);

            dbContext.Notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ListingId = message.ListingId,
                Channel = NotificationChannel.SignalR,
                SentAt = sentAt,
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Notified {UserCount} users about listing {ListingId}",
            usersToNotify.Count,
            message.ListingId);
    }
}

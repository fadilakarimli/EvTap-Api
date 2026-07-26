using EvTap.Modules.Notifications.Realtime;
using EvTap.Shared.Contracts;
using EvTap.Shared.Email;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace EvTap.Modules.Notifications.Consumers;

/// <summary>
/// Sends the payment receipt for a captured booking. Deliberately no Notification row here —
/// the same UserId+ListingId+Channel can legitimately recur (a renter booking the same listing
/// for two different date ranges), which would violate the unique index that guards against
/// double-notifying for listing matches. The renter's own booking history (GET
/// /api/bookings/mine) is the equivalent full record, same trade-off as MessageSentConsumer.
/// </summary>
public sealed class PaymentCompletedConsumer(
    IEmailSender emailSender,
    IHubContext<NotificationsHub> hubContext,
    ILogger<PaymentCompletedConsumer> logger) : IConsumer<PaymentCompletedEvent>
{
    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        var unitLabel = message.RentalType == "Hourly" ? "saat" : "gecə";
        var subject = $"EvTap — Ödəniş qəbzi: {message.ListingTitle}";
        var body =
            $"Ödənişiniz uğurla tamamlandı!\n\n" +
            $"Elan: {message.ListingTitle}\n" +
            $"Tarixlər: {message.StartAt:dd.MM.yyyy HH:mm} — {message.EndAt:dd.MM.yyyy HH:mm} ({message.Units} {unitLabel})\n" +
            $"Məbləğ: {message.TotalPriceAzn:N2} AZN\n" +
            $"Kart: **** **** **** {message.CardLast4}\n" +
            $"Əməliyyat nömrəsi: {message.PaymentReference}\n" +
            $"Ödəniş tarixi: {message.PaidAt:dd.MM.yyyy HH:mm}\n\n" +
            $"Bron nömrəniz: {message.BookingId}\n\n" +
            $"Təşəkkür edirik ki, EvTap-dan istifadə edirsiniz.";

        await emailSender.SendAsync(message.RenterEmail, subject, body, cancellationToken);

        await hubContext.Clients.User(message.RenterId.ToString()).SendAsync(
            "PaymentCompleted",
            new
            {
                message.BookingId,
                message.ListingId,
                message.ListingTitle,
                message.TotalPriceAzn,
            },
            cancellationToken);

        logger.LogInformation(
            "Sent payment receipt for booking {BookingId} to {RenterEmail}",
            message.BookingId,
            message.RenterEmail);
    }
}

namespace EvTap.Shared.Contracts;

/// <summary>Published by the Bookings module after the mock payment gateway approves a charge.
/// Consumed by the Notifications module to send a receipt email and push a SignalR event.</summary>
public sealed record PaymentCompletedEvent(
    Guid BookingId,
    Guid ListingId,
    string ListingTitle,
    Guid RenterId,
    string RenterEmail,
    Guid OwnerId,
    string RentalType,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    int Units,
    decimal UnitPriceAzn,
    decimal TotalPriceAzn,
    string PaymentReference,
    string CardLast4,
    DateTimeOffset PaidAt);

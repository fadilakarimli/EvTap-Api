namespace EvTap.Modules.Bookings.Domain;

public enum BookingStatus
{
    PendingPayment,
    Paid,
    Cancelled,
}

/// <summary>Whether a booking's <see cref="Booking.Units"/> counts hours or nights — mirrors
/// whichever of the listing's PricePerHour / PricePerNight the renter picked.</summary>
public enum RentalType
{
    Hourly,
    Nightly,
}

public sealed class Booking
{
    public Guid Id { get; init; }

    public Guid ListingId { get; init; }

    public required string ListingTitle { get; init; }

    public Guid RenterId { get; init; }

    public required string RenterEmail { get; init; }

    public Guid OwnerId { get; init; }

    public RentalType RentalType { get; init; }

    public DateTimeOffset StartAt { get; init; }

    public DateTimeOffset EndAt { get; init; }

    /// <summary>Number of hours (RentalType.Hourly) or nights (RentalType.Nightly) booked.</summary>
    public int Units { get; init; }

    /// <summary>Snapshot of the owner-set PricePerHour/PricePerNight (AZN) used to price this
    /// booking, so later edits to the listing's rates don't change past bookings.</summary>
    public decimal UnitPriceAzn { get; init; }

    public decimal TotalPriceAzn { get; init; }

    public BookingStatus Status { get; set; }

    /// <summary>Mock gateway transaction id, set once the charge succeeds.</summary>
    public string? PaymentReference { get; set; }

    /// <summary>Last 4 digits of the card used, for the receipt — never store full PANs.</summary>
    public string? CardLast4 { get; set; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? PaidAt { get; set; }
}

namespace EvTap.Modules.Listings.Domain;

public sealed class Listing
{
    public Guid Id { get; init; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public decimal Price { get; set; }

    /// <summary>Optional short-term rental rate, set by the owner. Null means the listing
    /// cannot be booked hourly via PayPal (see EvTap.Modules.Bookings).</summary>
    public decimal? PricePerHour { get; set; }

    /// <summary>Optional short-term rental rate, set by the owner. Null means the listing
    /// cannot be booked nightly via PayPal (see EvTap.Modules.Bookings).</summary>
    public decimal? PricePerNight { get; set; }

    public int Rooms { get; set; }

    public double AreaSquareMeters { get; set; }

    public required string District { get; set; }

    public required string Address { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public ListingStatus Status { get; set; }

    public Guid OwnerId { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public int ViewCount { get; set; }

    public List<ListingImage> Images { get; init; } = [];
}

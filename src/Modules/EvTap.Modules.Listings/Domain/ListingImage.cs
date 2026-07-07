namespace EvTap.Modules.Listings.Domain;

public sealed class ListingImage
{
    public Guid Id { get; init; }

    public Guid ListingId { get; init; }

    public required string Url { get; set; }

    public int Order { get; set; }
}

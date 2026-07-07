namespace EvTap.Modules.Listings.Domain;

public sealed class Favorite
{
    public Guid UserId { get; init; }

    public Guid ListingId { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}

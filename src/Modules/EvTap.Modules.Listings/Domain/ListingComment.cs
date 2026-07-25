namespace EvTap.Modules.Listings.Domain;

public sealed class ListingComment
{
    public Guid Id { get; init; }

    public Guid ListingId { get; init; }

    public Guid AuthorId { get; init; }

    /// <summary>Denormalized from the JWT name claim at write time, so reading comments never
    /// needs a cross-module query into the Users module.</summary>
    public required string AuthorName { get; init; }

    public required string Text { get; set; }

    public DateTimeOffset CreatedAt { get; init; }
}

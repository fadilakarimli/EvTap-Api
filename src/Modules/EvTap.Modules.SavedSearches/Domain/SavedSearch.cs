namespace EvTap.Modules.SavedSearches.Domain;

public sealed class SavedSearch
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public int? MinRooms { get; set; }

    public string? District { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; init; }
}

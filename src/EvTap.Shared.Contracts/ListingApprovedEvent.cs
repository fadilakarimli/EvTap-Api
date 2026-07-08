namespace EvTap.Shared.Contracts;

public sealed record ListingApprovedEvent(
    Guid ListingId,
    string Title,
    decimal Price,
    int Rooms,
    string District,
    Guid OwnerId,
    DateTimeOffset ApprovedAt);

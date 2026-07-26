using MediatR;

namespace EvTap.Shared.Contracts;

// Cross-module read-API contracts (records only). Handlers live inside the owning module;
// consumers dispatch these in-process via MediatR. This is the documented trade-off that keeps
// modules from referencing each other's projects while still allowing the Notifications
// consumer to query the SavedSearches/Users read models without a local read-model copy.
// Note: these intentionally return plain DTOs (not Result<T>) so this project keeps zero
// dependency on EvTap.Shared.

/// <summary>Owned by the SavedSearches module: active saved searches matching a listing.</summary>
public sealed record GetMatchingSavedSearchesQuery(
    decimal Price,
    int Rooms,
    string District) : IRequest<IReadOnlyList<MatchingSavedSearch>>;

public sealed record MatchingSavedSearch(Guid SavedSearchId, Guid UserId);

/// <summary>Owned by the Users module: resolve email addresses for a set of user ids.</summary>
public sealed record GetUserEmailsQuery(IReadOnlyList<Guid> UserIds)
    : IRequest<IReadOnlyDictionary<Guid, string>>;

/// <summary>Owned by the Listings module: the bare facts the Bookings module needs to price
/// and validate a booking, without referencing the Listings project directly.</summary>
public sealed record GetListingForBookingQuery(Guid ListingId) : IRequest<ListingForBooking?>;

public sealed record ListingForBooking(
    Guid Id,
    string Title,
    decimal? PricePerHour,
    decimal? PricePerNight,
    string Status,
    Guid OwnerId);

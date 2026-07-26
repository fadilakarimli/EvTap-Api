using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Listings.Features.GetListingForBooking;

/// <summary>
/// Handles the cross-module read query defined in EvTap.Shared.Contracts. Consumed in-process
/// by the Bookings module's CreateBookingHandler to price and validate a booking without
/// referencing the Listings project directly (same pattern as GetUserEmailsQuery).
/// </summary>
internal sealed class GetListingForBookingHandler(ListingsDbContext dbContext)
    : IRequestHandler<GetListingForBookingQuery, ListingForBooking?>
{
    public async Task<ListingForBooking?> Handle(GetListingForBookingQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Listings
            .Where(l => l.Id == request.ListingId)
            .Select(l => new ListingForBooking(
                l.Id, l.Title, l.PricePerHour, l.PricePerNight, l.Status.ToString(), l.OwnerId))
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);
    }
}

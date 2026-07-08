using EvTap.Modules.Listings.Domain;
using EvTap.Modules.Listings.Persistence;
using EvTap.Shared.Contracts;
using EvTap.Shared.Results;
using MassTransit;
using MediatR;

namespace EvTap.Modules.Listings.Features.ApproveListing;

internal sealed class ApproveListingHandler(ListingsDbContext dbContext, IPublishEndpoint publishEndpoint)
    : IRequestHandler<ApproveListingCommand, Result>
{
    public async Task<Result> Handle(ApproveListingCommand request, CancellationToken cancellationToken)
    {
        var listing = await dbContext.Listings.FindAsync([request.Id], cancellationToken);

        if (listing is null)
        {
            return Result.Failure(Error.NotFound("Listings.NotFound", "Listing not found."));
        }

        if (listing.Status != ListingStatus.Pending)
        {
            return Result.Failure(Error.Conflict(
                "Listings.NotPending",
                $"Only pending listings can be approved (current status: {listing.Status})."));
        }

        listing.Status = ListingStatus.Active;
        var approvedAt = DateTimeOffset.UtcNow;

        // With the EF outbox configured, this publish is written to the outbox table inside
        // the same transaction as the status change below — atomically.
        await publishEndpoint.Publish(
            new ListingApprovedEvent(
                listing.Id,
                listing.Title,
                listing.Price,
                listing.Rooms,
                listing.District,
                listing.OwnerId,
                approvedAt),
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

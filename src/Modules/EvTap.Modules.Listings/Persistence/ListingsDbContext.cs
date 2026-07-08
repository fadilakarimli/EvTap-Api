using EvTap.Modules.Listings.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Listings.Persistence;

public sealed class ListingsDbContext(DbContextOptions<ListingsDbContext> options) : DbContext(options)
{
    public DbSet<Listing> Listings => Set<Listing>();

    public DbSet<ListingImage> ListingImages => Set<ListingImage>();

    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("listings");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ListingsDbContext).Assembly);

        // MassTransit transactional outbox tables (live in the listings schema alongside
        // the aggregate they guard — the ListingApprovedEvent is written in the same
        // transaction as the Status update).
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}

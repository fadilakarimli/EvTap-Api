using EvTap.Modules.SavedSearches.Domain;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.SavedSearches.Persistence;

public sealed class SavedSearchesDbContext(DbContextOptions<SavedSearchesDbContext> options) : DbContext(options)
{
    public DbSet<SavedSearch> SavedSearches => Set<SavedSearch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("savedsearches");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SavedSearchesDbContext).Assembly);
    }
}

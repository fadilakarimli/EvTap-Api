using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EvTap.Modules.SavedSearches.Persistence;

internal sealed class SavedSearchesDbContextFactory : IDesignTimeDbContextFactory<SavedSearchesDbContext>
{
    public SavedSearchesDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SavedSearchesDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=evtapdb;Username=postgres;Password=postgres");

        return new SavedSearchesDbContext(optionsBuilder.Options);
    }
}

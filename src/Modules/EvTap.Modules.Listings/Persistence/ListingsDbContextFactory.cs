using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EvTap.Modules.Listings.Persistence;

internal sealed class ListingsDbContextFactory : IDesignTimeDbContextFactory<ListingsDbContext>
{
    public ListingsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ListingsDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=evtapdb;Username=postgres;Password=postgres");

        return new ListingsDbContext(optionsBuilder.Options);
    }
}

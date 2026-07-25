using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EvTap.Modules.Messaging.Persistence;

internal sealed class MessagingDbContextFactory : IDesignTimeDbContextFactory<MessagingDbContext>
{
    public MessagingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MessagingDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=evtapdb;Username=postgres;Password=postgres");

        return new MessagingDbContext(optionsBuilder.Options);
    }
}

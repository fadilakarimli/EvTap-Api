using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EvTap.Modules.Users.Persistence;

internal sealed class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=evtapdb;Username=postgres;Password=postgres");

        return new UsersDbContext(optionsBuilder.Options);
    }
}

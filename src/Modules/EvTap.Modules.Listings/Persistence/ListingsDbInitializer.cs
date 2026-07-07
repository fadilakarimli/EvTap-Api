using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EvTap.Modules.Listings.Persistence;

public static class ListingsDbInitializer
{
    public static async Task MigrateAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ListingsDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}

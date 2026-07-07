using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EvTap.Modules.SavedSearches.Persistence;

public static class SavedSearchesDbInitializer
{
    public static async Task MigrateAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<SavedSearchesDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}

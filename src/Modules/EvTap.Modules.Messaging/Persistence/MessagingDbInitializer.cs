using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EvTap.Modules.Messaging.Persistence;

public static class MessagingDbInitializer
{
    public static async Task MigrateAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MessagingDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}

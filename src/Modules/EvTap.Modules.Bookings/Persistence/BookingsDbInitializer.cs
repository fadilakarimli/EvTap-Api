using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EvTap.Modules.Bookings.Persistence;

public static class BookingsDbInitializer
{
    public static async Task MigrateAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<BookingsDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}

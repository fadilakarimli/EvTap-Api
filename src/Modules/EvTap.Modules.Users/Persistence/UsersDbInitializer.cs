using EvTap.Modules.Users.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvTap.Modules.Users.Persistence;

public static class UsersDbInitializer
{
    public static async Task MigrateAndSeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        await dbContext.Database.MigrateAsync();

        var adminEmail = configuration["Admin:Email"];
        var adminPassword = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var adminExists = await dbContext.Users.AnyAsync(u => u.Role == UserRole.Admin);
        if (adminExists)
        {
            return;
        }

        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Email = adminEmail,
            PasswordHash = string.Empty,
            Role = UserRole.Admin,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        admin.PasswordHash = passwordHasher.HashPassword(admin, adminPassword);

        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync();
    }
}

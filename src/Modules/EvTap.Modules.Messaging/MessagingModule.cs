using EvTap.Modules.Messaging.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvTap.Modules.Messaging;

public static class MessagingModule
{
    public static IServiceCollection AddMessagingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MessagingModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(MessagingModule).Assembly, includeInternalTypes: true);

        services.AddDbContext<MessagingDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("evtapdb")));

        return services;
    }
}

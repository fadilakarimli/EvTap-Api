using EvTap.Modules.Listings.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvTap.Modules.Listings;

public static class ListingsModule
{
    public static IServiceCollection AddListingsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ListingsModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(ListingsModule).Assembly, includeInternalTypes: true);

        services.AddDbContext<ListingsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("evtapdb")));

        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration.GetConnectionString("redis"));

        services.AddHybridCache();

        return services;
    }
}

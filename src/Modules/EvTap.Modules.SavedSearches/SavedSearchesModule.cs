using EvTap.Modules.SavedSearches.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvTap.Modules.SavedSearches;

public static class SavedSearchesModule
{
    public static IServiceCollection AddSavedSearchesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SavedSearchesModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(SavedSearchesModule).Assembly, includeInternalTypes: true);

        services.AddDbContext<SavedSearchesDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("evtapdb")));

        return services;
    }
}

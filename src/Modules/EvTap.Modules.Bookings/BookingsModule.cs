using EvTap.Modules.Bookings.Infrastructure;
using EvTap.Modules.Bookings.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvTap.Modules.Bookings;

public static class BookingsModule
{
    public static IServiceCollection AddBookingsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BookingsModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(BookingsModule).Assembly, includeInternalTypes: true);

        services.AddDbContext<BookingsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("evtapdb")));

        services.AddSingleton<IPaymentGateway, MockPaymentGateway>();

        return services;
    }
}

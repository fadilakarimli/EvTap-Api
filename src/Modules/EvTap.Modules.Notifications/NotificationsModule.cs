using EvTap.Modules.Notifications.Email;
using EvTap.Modules.Notifications.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EvTap.Modules.Notifications;

public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(NotificationsModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(NotificationsModule).Assembly, includeInternalTypes: true);

        services.AddDbContext<NotificationsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("evtapdb")));

        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));

        // Log-only sender in dev (no SMTP host configured); real MailKit sender otherwise.
        var smtpHost = configuration[$"{SmtpOptions.SectionName}:Host"];
        if (string.IsNullOrWhiteSpace(smtpHost))
        {
            services.AddSingleton<IEmailSender, LogOnlyEmailSender>();
        }
        else
        {
            services.AddSingleton<IEmailSender, MailKitEmailSender>();
        }

        return services;
    }
}

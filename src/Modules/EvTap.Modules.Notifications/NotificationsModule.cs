using EvTap.Modules.Notifications.Email;
using EvTap.Modules.Notifications.Persistence;
using EvTap.Shared.Email;
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

        // Real MailKit sender only when the SMTP credentials are fully filled in; otherwise a
        // log-only sender (codes/emails show up in the API console) so nothing breaks in dev.
        var smtpHost = configuration[$"{SmtpOptions.SectionName}:Host"];
        var smtpUsername = configuration[$"{SmtpOptions.SectionName}:Username"];
        var smtpPassword = configuration[$"{SmtpOptions.SectionName}:Password"];

        if (string.IsNullOrWhiteSpace(smtpHost) ||
            string.IsNullOrWhiteSpace(smtpUsername) ||
            string.IsNullOrWhiteSpace(smtpPassword))
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

using EvTap.Shared.Email;
using Microsoft.Extensions.Logging;

namespace EvTap.Modules.Notifications.Email;

/// <summary>
/// Development email sender: writes the would-be email to the log instead of sending it.
/// Registered when no SMTP host is configured.
/// </summary>
internal sealed class LogOnlyEmailSender(ILogger<LogOnlyEmailSender> logger) : IEmailSender
{
    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "EMAIL (log-only) To: {To} | Subject: {Subject} | Body: {Body}",
            to,
            subject,
            body);

        return Task.CompletedTask;
    }
}

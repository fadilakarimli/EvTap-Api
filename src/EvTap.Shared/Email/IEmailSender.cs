namespace EvTap.Shared.Email;

/// <summary>
/// Cross-module email abstraction. Implementations (MailKit / log-only for dev) live in the
/// Notifications module and are registered into the shared container there; other modules
/// (e.g. Users, for OTP codes) depend only on this interface.
/// </summary>
public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}

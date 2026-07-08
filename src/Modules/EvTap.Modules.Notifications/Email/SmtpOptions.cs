namespace EvTap.Modules.Notifications.Email;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string? Host { get; init; }

    public int Port { get; init; } = 25;

    public string? Username { get; init; }

    public string? Password { get; init; }

    public string FromAddress { get; init; } = "noreply@evtap.local";

    public string FromName { get; init; } = "EvTap";
}

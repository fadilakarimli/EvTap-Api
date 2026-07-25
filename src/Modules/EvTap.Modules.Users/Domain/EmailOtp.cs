namespace EvTap.Modules.Users.Domain;

/// <summary>A short-lived 6-digit code sent to a user's email to verify ownership.</summary>
public sealed class EmailOtp
{
    public Guid Id { get; init; }

    public required string Email { get; init; }

    public required string Code { get; init; }

    public DateTimeOffset ExpiresAt { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}

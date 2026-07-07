namespace EvTap.Modules.Users.Domain;

public sealed class User
{
    public Guid Id { get; init; }

    public required string Name { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public UserRole Role { get; set; }

    public DateTimeOffset CreatedAt { get; init; }
}

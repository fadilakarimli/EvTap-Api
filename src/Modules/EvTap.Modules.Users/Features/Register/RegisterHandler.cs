using EvTap.Modules.Users.Domain;
using EvTap.Modules.Users.Persistence;
using EvTap.Modules.Users.Security;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Users.Features.Register;

internal sealed class RegisterHandler(
    UsersDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    OtpService otpService) : IRequestHandler<RegisterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailInUse = await dbContext.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailInUse)
        {
            return Result.Failure<Guid>(Error.Conflict("Users.EmailInUse", "A user with this email already exists."));
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = string.Empty,
            Role = UserRole.User,
            IsEmailVerified = false,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Email ownership must be proven before the account can log in: send a 6-digit code.
        await otpService.IssueAsync(user.Email, cancellationToken);

        return Result.Success(user.Id);
    }
}

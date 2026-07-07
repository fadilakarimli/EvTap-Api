using EvTap.Modules.Users.Domain;
using EvTap.Modules.Users.Persistence;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Users.Features.Register;

internal sealed class RegisterHandler(UsersDbContext dbContext, IPasswordHasher<User> passwordHasher)
    : IRequestHandler<RegisterCommand, Result<Guid>>
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
            CreatedAt = DateTimeOffset.UtcNow,
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}

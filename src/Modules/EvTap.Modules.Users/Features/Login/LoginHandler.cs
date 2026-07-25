using EvTap.Modules.Users.Domain;
using EvTap.Modules.Users.Persistence;
using EvTap.Modules.Users.Security;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EvTap.Modules.Users.Features.Login;

internal sealed class LoginHandler(
    UsersDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    TokenGenerator tokenGenerator,
    IOptions<JwtOptions> jwtOptions) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<LoginResponse>(Error.Unauthorized("Users.InvalidCredentials", "Invalid email or password."));
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Result.Failure<LoginResponse>(Error.Unauthorized("Users.InvalidCredentials", "Invalid email or password."));
        }

        if (!user.IsEmailVerified)
        {
            return Result.Failure<LoginResponse>(Error.Unauthorized(
                "Users.EmailNotVerified",
                "Email hələ təsdiqlənməyib — emailinizə göndərilən 6 rəqəmli kodu daxil edin."));
        }

        var token = tokenGenerator.GenerateToken(user);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpirationMinutes);

        return Result.Success(new LoginResponse(token, expiresAtUtc));
    }
}

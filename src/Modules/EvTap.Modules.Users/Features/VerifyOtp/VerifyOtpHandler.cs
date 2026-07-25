using EvTap.Modules.Users.Features.Login;
using EvTap.Modules.Users.Persistence;
using EvTap.Modules.Users.Security;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EvTap.Modules.Users.Features.VerifyOtp;

internal sealed class VerifyOtpHandler(
    UsersDbContext dbContext,
    TokenGenerator tokenGenerator,
    IOptions<JwtOptions> jwtOptions) : IRequestHandler<VerifyOtpCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<LoginResponse>(
                Error.NotFound("Users.NotFound", "Bu email ilə istifadəçi tapılmadı."));
        }

        if (!user.IsEmailVerified)
        {
            var otp = await dbContext.EmailOtps
                .SingleOrDefaultAsync(o => o.Email == request.Email && o.Code == request.Code, cancellationToken);

            if (otp is null || otp.ExpiresAt < DateTimeOffset.UtcNow)
            {
                return Result.Failure<LoginResponse>(
                    Error.Validation("Users.InvalidOtp", "Kod yanlışdır və ya vaxtı bitib. Yenidən göndərməyi yoxlayın."));
            }

            user.IsEmailVerified = true;
            dbContext.EmailOtps.Remove(otp);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var token = tokenGenerator.GenerateToken(user);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpirationMinutes);

        return Result.Success(new LoginResponse(token, expiresAtUtc));
    }
}

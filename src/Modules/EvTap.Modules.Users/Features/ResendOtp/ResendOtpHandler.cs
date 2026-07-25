using EvTap.Modules.Users.Persistence;
using EvTap.Modules.Users.Security;
using EvTap.Shared.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvTap.Modules.Users.Features.ResendOtp;

internal sealed class ResendOtpHandler(UsersDbContext dbContext, OtpService otpService)
    : IRequestHandler<ResendOtpCommand, Result>
{
    public async Task<Result> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure(Error.NotFound("Users.NotFound", "Bu email ilə istifadəçi tapılmadı."));
        }

        if (user.IsEmailVerified)
        {
            return Result.Failure(Error.Conflict("Users.AlreadyVerified", "Bu email artıq təsdiqlənib."));
        }

        await otpService.IssueAsync(user.Email, cancellationToken);

        return Result.Success();
    }
}

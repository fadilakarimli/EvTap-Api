using System.Security.Cryptography;
using EvTap.Modules.Users.Domain;
using EvTap.Modules.Users.Persistence;
using EvTap.Shared.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EvTap.Modules.Users.Security;

/// <summary>Generates, stores and emails 6-digit verification codes (valid for 10 minutes).
/// Old codes for the same email are replaced so only the latest code works.</summary>
internal sealed class OtpService(UsersDbContext dbContext, IEmailSender emailSender, ILogger<OtpService> logger)
{
    public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(10);

    public async Task IssueAsync(string email, CancellationToken cancellationToken)
    {
        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

        await dbContext.EmailOtps
            .Where(o => o.Email == email)
            .ExecuteDeleteAsync(cancellationToken);

        dbContext.EmailOtps.Add(new EmailOtp
        {
            Id = Guid.NewGuid(),
            Email = email,
            Code = code,
            ExpiresAt = DateTimeOffset.UtcNow.Add(Lifetime),
            CreatedAt = DateTimeOffset.UtcNow,
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        // The code is already persisted at this point, so a delivery failure (bad SMTP creds,
        // transient DNS hiccup, provider outage) must not fail registration/resend and leave
        // the account stuck: log it and let the user retry via "resend code".
        try
        {
            await emailSender.SendAsync(
                email,
                "EvTap — email təsdiq kodu",
                $"EvTap qeydiyyatınızı tamamlamaq üçün təsdiq kodunuz: {code}\n\n" +
                $"Kod {Lifetime.TotalMinutes:0} dəqiqə ərzində etibarlıdır. " +
                "Əgər bu sorğunu siz etməmisinizsə, bu mesajı nəzərə almayın.",
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send OTP email to {Email}", email);
        }
    }
}

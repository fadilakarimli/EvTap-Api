using FluentValidation;

namespace EvTap.Modules.Users.Features.VerifyOtp;

internal sealed class VerifyOtpValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).NotEmpty().Length(6).Matches("^[0-9]{6}$")
            .WithMessage("Kod 6 rəqəmdən ibarət olmalıdır.");
    }
}

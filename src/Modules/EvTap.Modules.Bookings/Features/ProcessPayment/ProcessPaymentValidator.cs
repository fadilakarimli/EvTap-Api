using FluentValidation;

namespace EvTap.Modules.Bookings.Features.ProcessPayment;

internal sealed class ProcessPaymentValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentValidator()
    {
        RuleFor(x => x.CardNumber).NotEmpty();
        RuleFor(x => x.CardholderName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ExpiryMonth).NotEmpty().Matches("^(0[1-9]|1[0-2])$").WithMessage("Ay 01-12 arasında olmalıdır.");
        RuleFor(x => x.ExpiryYear).NotEmpty().Matches("^[0-9]{2,4}$");
        RuleFor(x => x.Cvv).NotEmpty();
    }
}

using EvTap.Modules.Bookings.Domain;
using FluentValidation;

namespace EvTap.Modules.Bookings.Features.CreateBooking;

internal sealed class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.RentalType)
            .Must(value => Enum.TryParse<RentalType>(value, ignoreCase: true, out _))
            .WithMessage("RentalType 'Hourly' və ya 'Nightly' olmalıdır.");

        RuleFor(x => x.StartAt)
            .GreaterThanOrEqualTo(_ => DateTimeOffset.UtcNow.AddMinutes(-1))
            .WithMessage("Başlama vaxtı keçmişdə ola bilməz.");

        RuleFor(x => x.Units)
            .GreaterThan(0)
            .LessThanOrEqualTo(720)
            .WithMessage("Müddət 1 ilə 720 arasında olmalıdır.");
    }
}

using FluentValidation;

namespace EvTap.Modules.Messaging.Features.SendMessage;

internal sealed class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.ListingTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RecipientId)
            .NotEqual(x => x.SenderId)
            .WithMessage("You cannot send a message to yourself.");
    }
}

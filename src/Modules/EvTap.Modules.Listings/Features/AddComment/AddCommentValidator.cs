using FluentValidation;

namespace EvTap.Modules.Listings.Features.AddComment;

internal sealed class AddCommentValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
    }
}

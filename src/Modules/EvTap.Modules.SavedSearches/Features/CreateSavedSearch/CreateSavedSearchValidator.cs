using FluentValidation;

namespace EvTap.Modules.SavedSearches.Features.CreateSavedSearch;

internal sealed class CreateSavedSearchValidator : AbstractValidator<CreateSavedSearchCommand>
{
    public CreateSavedSearchValidator()
    {
        RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue);
        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue);
        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(x => x.MinPrice!.Value)
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
            .WithMessage("'MaxPrice' must be greater than or equal to 'MinPrice'.");
        RuleFor(x => x.MinRooms).GreaterThan(0).When(x => x.MinRooms.HasValue);
        RuleFor(x => x.District).MaximumLength(100);
        RuleFor(x => x)
            .Must(x => x.MinPrice.HasValue || x.MaxPrice.HasValue || x.MinRooms.HasValue || !string.IsNullOrWhiteSpace(x.District))
            .WithMessage("At least one filter (MinPrice, MaxPrice, MinRooms or District) must be provided.");
    }
}

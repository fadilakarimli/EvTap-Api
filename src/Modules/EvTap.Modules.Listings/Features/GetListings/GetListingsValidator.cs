using FluentValidation;

namespace EvTap.Modules.Listings.Features.GetListings;

internal sealed class GetListingsValidator : AbstractValidator<GetListingsQuery>
{
    public GetListingsValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue);
        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue);
        RuleFor(x => x.Rooms).GreaterThan(0).When(x => x.Rooms.HasValue);
    }
}

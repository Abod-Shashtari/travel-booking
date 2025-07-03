using FluentValidation;

namespace TravelBooking.Application.Discounts.GetDiscounts;

public class GetDiscountsQueryValidator:AbstractValidator<GetDiscountsQuery>
{
    public GetDiscountsQueryValidator()
    {
        RuleFor(x=>x.PageNumber).GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");
    }
}
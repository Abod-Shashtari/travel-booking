using FluentValidation;
using TravelBooking.Application.Hotels.SearchHotels;

namespace TravelBooking.Application.Hotels.GetHotels;

public class GetHotelsQueryValidator:AbstractValidator<SearchHotelsQuery>
{
    public GetHotelsQueryValidator()
    {
        RuleFor(x=>x.PageNumber).GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");
    }
}
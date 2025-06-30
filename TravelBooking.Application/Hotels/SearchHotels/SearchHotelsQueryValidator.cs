using FluentValidation;

namespace TravelBooking.Application.Hotels.SearchHotels;

public class SearchHotelsQueryValidator:AbstractValidator<SearchHotelsQuery>
{
    public SearchHotelsQueryValidator()
    {
        RuleFor(x=>x.PageNumber).GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");
        When(x => x.HotelFilter != null, () =>
        {
            RuleFor(x => x.HotelFilter!.CheckIn)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("CheckIn must be Now or in future date");
            RuleFor(x => x.HotelFilter!.CheckOut)
                .GreaterThan(x=>x.HotelFilter!.CheckIn)
                .WithMessage("CheckOut must be Grater than CheckIn date");
        });
    }
}
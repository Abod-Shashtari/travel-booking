using FluentValidation;
using TravelBooking.Application.Cities.GetCities;

namespace TravelBooking.Application.Amenities.GetAmenities;

public class GetAmenitiesQueryValidator:AbstractValidator<GetCitiesQuery>
{
    public GetAmenitiesQueryValidator()
    {
        RuleFor(x=>x.PageNumber).GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");
    }
}
using FluentValidation;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.CreateHotels;

public class CreateHotelCommandValidator:AbstractValidator<CreateHotelCommand>
{
    public CreateHotelCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Location.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90 degrees.");
        RuleFor(x => x.Location.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180 degrees.");
        RuleFor(x => x.CityId).NotEmpty();
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}
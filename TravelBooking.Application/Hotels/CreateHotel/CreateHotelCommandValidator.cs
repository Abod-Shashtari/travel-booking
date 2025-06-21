using FluentValidation;

namespace TravelBooking.Application.Hotels.CreateHotel;

public class CreateHotelCommandValidator:AbstractValidator<CreateHotelCommand>
{
    public CreateHotelCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters");
        RuleFor(x => x.Location.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90 degrees.");
        RuleFor(x => x.Location.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180 degrees.");
        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters");
        });
        RuleFor(x => x.CityId).NotEmpty();
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}
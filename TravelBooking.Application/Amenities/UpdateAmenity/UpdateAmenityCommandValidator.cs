using FluentValidation;

namespace TravelBooking.Application.Cities.UpdateCity;

public class UpdateAmenityCommandValidator:AbstractValidator<UpdateCityCommand>
{
    public UpdateAmenityCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        RuleFor(x => x.Country).NotEmpty()
            .MaximumLength(100).WithMessage("Country Name must not exceed 100 characters");
        RuleFor(x => x.PostOffice).NotEmpty()
            .MaximumLength(20).WithMessage("Post Office must not exceed 20 characters");
    }
}
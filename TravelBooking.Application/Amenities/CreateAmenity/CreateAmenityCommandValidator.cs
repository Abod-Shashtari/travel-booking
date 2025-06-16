using FluentValidation;

namespace TravelBooking.Application.Amenities.CreateAmenity;

public class CreateAmenityCommandValidator:AbstractValidator<CreateAmenityCommand>
{
    public CreateAmenityCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters");
        });
    }
}
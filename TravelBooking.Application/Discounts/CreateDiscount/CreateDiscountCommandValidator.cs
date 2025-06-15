using FluentValidation;
using TravelBooking.Application.Cities.CreateCity;

namespace TravelBooking.Application.Discounts.CreateDiscount;

public class CreateDiscountCommandValidator:AbstractValidator<CreateDiscountCommand>
{
    public CreateDiscountCommandValidator()
    {
        // RuleFor(x => x.Name).NotEmpty()
        //     .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        // RuleFor(x => x.Country).NotEmpty()
        //     .MaximumLength(100).WithMessage("Country Name must not exceed 100 characters");
        // RuleFor(x => x.PostOffice).NotEmpty()
        //     .MaximumLength(20).WithMessage("Post Office must not exceed 20 characters");
    }
}
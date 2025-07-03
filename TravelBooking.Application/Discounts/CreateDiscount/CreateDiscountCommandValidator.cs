using FluentValidation;
using TravelBooking.Application.Cities.CreateCity;

namespace TravelBooking.Application.Discounts.CreateDiscount;

public class CreateDiscountCommandValidator:AbstractValidator<CreateDiscountCommand>
{
    public CreateDiscountCommandValidator()
    {
        RuleFor(x => x.RoomTypeId).NotEmpty();
        
        RuleFor(x => x.Percentage)
            .NotEmpty().WithMessage("Please specify a percentage")
            .InclusiveBetween(1,100)
            .WithMessage("The Percentage must be between 1 and 100");
        
        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("start date must be Now or in future date");
        RuleFor(x => x.EndDate)
            .GreaterThan(x=>x.StartDate)
            .WithMessage("end date must be Grater than start date");
    }
}
using FluentValidation;

namespace TravelBooking.Application.Bookings.CreateBooking;

public class CreateBookingsCommandValidator:AbstractValidator<CreateBookingCommand>
{
    public CreateBookingsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.HotelId).NotEmpty();
        RuleFor(x => x.CheckIn)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("CheckIn must be Now or in future date");
        RuleFor(x => x.CheckOut)
            .GreaterThan(x=>x.CheckIn)
            .WithMessage("CheckOut must be Grater than CheckIn date");
    }
}
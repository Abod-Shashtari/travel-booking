using FluentValidation;
using TravelBooking.Application.RoomTypes.CreateRoomType;

namespace TravelBooking.Application.RoomTypes.UpdateRoomType;

public class UpdateRoomTypeCommandValidator:AbstractValidator<UpdateRoomTypeCommand>
{
    public UpdateRoomTypeCommandValidator()
    {
        RuleFor(x=>x.RoomTypeId).NotEmpty();
        RuleFor(x => x.HotelId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty()
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters");
        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description).NotEmpty()
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters");
        });
        RuleFor(x => x.PricePerNight).NotEmpty()
            .GreaterThan(0)
            .WithMessage("Price per night must be greater than zero");
    }
}
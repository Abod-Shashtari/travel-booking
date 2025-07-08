using FluentValidation;

namespace TravelBooking.Application.Rooms.UpdateRoom;

public class UpdateRoomCommandValidator:AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomCommandValidator()
    {
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.Number).NotEmpty()
            .MaximumLength(10)
            .WithMessage("Room Number must be less or equal to 10 characters");
        RuleFor(x => x.RoomTypeId).NotEmpty();
        RuleFor(x => x.AdultCapacity).NotEmpty()
            .InclusiveBetween(1,10)
            .WithMessage("Adult capacity must be between 1 and 10");
        RuleFor(x => x.ChildrenCapacity)
            .InclusiveBetween(0,10)
            .WithMessage("Children capacity must be between 0 and 10");
    }
}
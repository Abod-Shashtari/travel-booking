using FluentValidation;

namespace TravelBooking.Application.RoomTypes.GetRoomTypesOfHotel;

public class GetRoomTypesOfHotelQueryValidator:AbstractValidator<GetRoomTypesOfHotelQuery>
{
    public GetRoomTypesOfHotelQueryValidator()
    {
        RuleFor(x=>x.HotelId).NotEmpty()
            .WithMessage("hotel Id is required.");
        RuleFor(x=>x.PageNumber).GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");
    }
}
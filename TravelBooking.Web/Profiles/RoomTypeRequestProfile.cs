using AutoMapper;
using TravelBooking.Application.RoomTypes.CreateRoomType;
using TravelBooking.Application.RoomTypes.GetRoomTypes;
using TravelBooking.Application.RoomTypes.GetRoomTypesOfHotel;
using TravelBooking.Application.RoomTypes.UpdateRoomType;
using TravelBooking.Web.Requests.RoomTypes;

namespace TravelBooking.Web.Profiles;

public class RoomTypeRequestProfile:Profile
{
    public RoomTypeRequestProfile()
    {
        CreateMap<GetRoomTypesRequest, GetRoomTypesQuery>();
        CreateMap<GetRoomTypesOfHotelRequest, GetRoomTypesOfHotelQuery>()
            .ForCtorParam(
                "HotelId",
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            );
        CreateMap<CreateRoomTypeRequest, CreateRoomTypeCommand>();
        CreateMap<UpdateRoomTypeRequest, UpdateRoomTypeCommand>()
            .ForCtorParam(
                "RoomTypeId",
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            );
    }
}
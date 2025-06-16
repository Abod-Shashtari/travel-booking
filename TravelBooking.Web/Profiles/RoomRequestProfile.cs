using AutoMapper;
using TravelBooking.Application.Rooms.CreateRoom;
using TravelBooking.Application.Rooms.GetRooms;
using TravelBooking.Application.Rooms.UpdateRoom;
using TravelBooking.Web.Requests.Rooms;

namespace TravelBooking.Web.Profiles;

public class RoomRequestProfile:Profile
{
    public RoomRequestProfile()
    {
        CreateMap<CreateRoomRequest, CreateRoomCommand>()
            .ForCtorParam(
                "RoomTypeId",
                opt => opt.MapFrom((src, ctx) => Guid.Empty));
        CreateMap<GetRoomsRequest, GetRoomsQuery>()
            .ForCtorParam(
                "RoomTypeId",
                opt => opt.MapFrom((src, ctx) => Guid.Empty));
        CreateMap<UpdateRoomRequest, UpdateRoomCommand>()
            .ForCtorParam(
                "RoomId",
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            );
    }
}
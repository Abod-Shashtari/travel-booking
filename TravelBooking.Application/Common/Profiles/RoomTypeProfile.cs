using AutoMapper;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.RoomTypes.CreateRoomType;
using TravelBooking.Application.RoomTypes.UpdateRoomType;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class RoomTypeProfile:Profile
{
    public RoomTypeProfile()
    {
        CreateMap<CreateRoomTypeCommand, RoomType>();
        CreateMap<UpdateRoomTypeCommand, RoomType>();
        CreateMap<RoomType,RoomTypeResponse>();
    }
}
using AutoMapper;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Rooms.CreateRoom;
using TravelBooking.Application.Rooms.UpdateRoom;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class RoomProfile:Profile
{
    public RoomProfile()
    {
        CreateMap<CreateRoomCommand, Room>();
        CreateMap<UpdateRoomCommand, Room>();
        CreateMap<Room, RoomResponse>();
    }
}
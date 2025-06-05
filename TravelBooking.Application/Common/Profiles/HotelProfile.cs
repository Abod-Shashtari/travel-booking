using AutoMapper;
using TravelBooking.Application.Hotels.CreateHotels;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class HotelProfile:Profile
{
    public HotelProfile()
    {
        CreateMap<CreateHotelCommand,Hotel>();
    }
}
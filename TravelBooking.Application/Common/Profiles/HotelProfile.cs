using AutoMapper;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Hotels.CreateHotels;
using TravelBooking.Application.Hotels.UpdateHotel;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class HotelProfile:Profile
{
    public HotelProfile()
    {
        CreateMap<CreateHotelCommand, Hotel>();
        CreateMap<UpdateHotelCommand, Hotel>();
        CreateMap<Hotel, HotelResponse>();
    }
}
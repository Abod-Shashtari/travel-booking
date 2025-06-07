using AutoMapper;
using TravelBooking.Application.Hotels.CreateHotels;
using TravelBooking.Application.Hotels.SearchHotel;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class HotelProfile:Profile
{
    public HotelProfile()
    {
        CreateMap<CreateHotelCommand, Hotel>();
        CreateMap<SearchHotelRequest, HotelFilter>();
        CreateMap<SearchHotelRequest, SearchHotelQuery>()
        .ForCtorParam("HotelFilter", opt => opt.MapFrom(src => src))
        .ForCtorParam("PageNumber", opt => opt.MapFrom(src => src.PageNumber))
        .ForCtorParam("PageSize", opt => opt.MapFrom(src => src.PageSize));
    }
}
using AutoMapper;
using TravelBooking.Application.Hotels.CreateHotels;
using TravelBooking.Application.Hotels.SearchHotel;
using TravelBooking.Web.Requests.Hotels;

namespace TravelBooking.Web.Profiles;

public class HotelProfile:Profile
{
    public HotelProfile()
    {
        CreateMap<CreateHotelRequest, CreateHotelCommand>();
        CreateMap<SearchHotelRequest, HotelFilter>();
        CreateMap<SearchHotelRequest, SearchHotelQuery>()
        .ForCtorParam("HotelFilter", opt => opt.MapFrom(src => src))
        .ForCtorParam("PageNumber", opt => opt.MapFrom(src => src.PageNumber))
        .ForCtorParam("PageSize", opt => opt.MapFrom(src => src.PageSize));
    }
}
using AutoMapper;
using TravelBooking.Application.Hotels.CreateHotels;
using TravelBooking.Application.Hotels.GetHotels;
using TravelBooking.Application.Hotels.UpdateHotel;
using TravelBooking.Web.Requests.Hotels;

namespace TravelBooking.Web.Profiles;

public class HotelRequestProfile:Profile
{
    public HotelRequestProfile()
    {
        CreateMap<CreateHotelRequest, CreateHotelCommand>();
        CreateMap<UpdateHotelRequest, UpdateHotelCommand>()
            .ForCtorParam(
                "HotelId",
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            );
        CreateMap<GetHotelsRequest, HotelFilter>();
        CreateMap<GetHotelsRequest, GetHotelsQuery>()
        .ForCtorParam("HotelFilter", opt => opt.MapFrom(src => src))
        .ForCtorParam("PageNumber", opt => opt.MapFrom(src => src.PageNumber))
        .ForCtorParam("PageSize", opt => opt.MapFrom(src => src.PageSize));
    }
}
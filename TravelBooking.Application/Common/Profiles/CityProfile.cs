using AutoMapper;
using TravelBooking.Application.Cities.CreateCity;
using TravelBooking.Application.Cities.UpdateCity;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Cities.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class CityProfile:Profile
{
    public CityProfile()
    {
        CreateMap<CreateCityCommand, City>();
        CreateMap<UpdateCityCommand, City>();
        CreateMap<City, CityResponse>()
            .ForCtorParam("NumberOfHotels", opt => opt.MapFrom(src => src.Hotels.Count))
            .ForCtorParam("Thumbnail", opt => opt.MapFrom(src => new ImageResponse(null,"")));
    }
}
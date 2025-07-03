using AutoMapper;
using TravelBooking.Application.Cities.CreateCity;
using TravelBooking.Application.Cities.GetCities;
using TravelBooking.Application.Cities.UpdateCity;
using TravelBooking.Web.Requests.Cities;

namespace TravelBooking.Web.Profiles;

public class CityRequestProfile:Profile
{
    public CityRequestProfile()
    {
        CreateMap<GetCitiesRequest, GetCitiesQuery>();
        CreateMap<CreateCityRequest, CreateCityCommand>();
        CreateMap<UpdateCityRequest, UpdateCityCommand>()
            .ForCtorParam(
                "CityId", 
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            );
    }
}

using AutoMapper;
using TravelBooking.Application.Cities.CreateCity;
using TravelBooking.Web.Requests.Cities;

namespace TravelBooking.Web.Profiles;

public class CityProfile:Profile
{
    public CityProfile()
    {
        CreateMap<CreateCityRequest, CreateCityCommand>();
    }
}

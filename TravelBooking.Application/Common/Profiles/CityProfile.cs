using AutoMapper;
using TravelBooking.Application.Cities.CreateCity;
using TravelBooking.Domain.Cities.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class CityProfile:Profile
{
    public CityProfile()
    {
        CreateMap<CreateCityCommand, City>();
    }
}
using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Cities.GetCity;

public class GetCityQueryHandler:IRequestHandler<GetCityQuery,Result<CityResponse?>>
{
    private readonly IRepository<City> _cityRepository;
    
    public GetCityQueryHandler(IRepository<City> cityRepository)
    {
        _cityRepository = cityRepository;
    }
    public async Task<Result<CityResponse?>> Handle(GetCityQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<City, CityResponse>> selector = city => new CityResponse(
            city.Id,
            city.Name,
            city.Country,
            city.PostOffice,
            city.Hotels.Count
        );
        var cityResponse = await _cityRepository.GetByIdAsync(request.CityId, selector, cancellationToken);
        if (cityResponse == null) return Result<CityResponse?>.Failure(CityErrors.CityNotFound());
        return Result<CityResponse?>.Success(cityResponse);
    }
}
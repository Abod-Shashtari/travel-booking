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
    private readonly IMapper _mapper;
    
    public GetCityQueryHandler(IRepository<City> cityRepository, IMapper mapper)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
    }
    public async Task<Result<CityResponse?>> Handle(GetCityQuery request, CancellationToken cancellationToken)
    {
        var city = await _cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city == null) return Result<CityResponse?>.Failure(CityErrors.CityNotFound());
        var cityResponse = _mapper.Map<CityResponse>(city);
        return Result<CityResponse?>.Success(cityResponse);
    }
}
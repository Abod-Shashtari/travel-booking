using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Cities.GetCities;

public class GetCitiesQueryHandler:IRequestHandler<GetCitiesQuery, Result<PaginatedList<CityResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IRepository<City> _cityRepository;

    public GetCitiesQueryHandler(IMapper mapper, IRepository<City> cityRepository)
    {
        _mapper = mapper;
        _cityRepository = cityRepository;
    }

    public async Task<Result<PaginatedList<CityResponse>>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        var cities= await _cityRepository.GetPaginatedListAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        var mappedItems = _mapper.Map<List<CityResponse>>(cities.Data);
        var citiesResponse = new PaginatedList<CityResponse>(mappedItems, cities.TotalCount, request.PageSize, request.PageNumber);
        
        return Result<PaginatedList<CityResponse>>.Success(citiesResponse);
    }
}
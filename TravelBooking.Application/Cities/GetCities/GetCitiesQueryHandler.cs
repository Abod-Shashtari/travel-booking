using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Cities.GetCities;

public class GetCitiesQueryHandler:IRequestHandler<GetCitiesQuery, Result<PaginatedList<CityResponse>>>
{
    private readonly IRepository<City> _cityRepository;

    public GetCitiesQueryHandler(IRepository<City> cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<Result<PaginatedList<CityResponse>>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        var specification = new PaginationSpecification<City>(request.PageNumber, request.PageSize,city=>city.CreatedAt,true);
        Expression<Func<City, CityResponse>> selector = city => new CityResponse(
            city.Id,
            city.Name,
            city.Country,
            city.PostOffice,
            city.Hotels.Count
        );
        var cities= await _cityRepository.GetPaginatedListAsync(
            specification,
            selector,
            cancellationToken
        );
        
        return Result<PaginatedList<CityResponse>>.Success(cities);
    }
}
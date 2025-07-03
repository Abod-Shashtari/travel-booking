using System.Linq.Expressions;
using MediatR;
using TravelBooking.Application.Cities.Specifications;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Cities.GetTrendingCities;

public class GetTrendingCitiesQueryHandler:IRequestHandler<GetTrendingCitiesQuery,Result<PaginatedList<CityResponse>>>
{
    private readonly IRepository<City> _cityRepository;

    public GetTrendingCitiesQueryHandler(IRepository<City> cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<Result<PaginatedList<CityResponse>>> Handle(GetTrendingCitiesQuery request, CancellationToken cancellationToken)
    {
        var specification = new GetTrendingCitiesSpecification(5);
        
        Expression<Func<City, CityResponse>> selector = city => new CityResponse(
            city.Id,
            city.Name,
            city.Country,
            city.PostOffice,
            city.Hotels.Count,
            new ImageResponse(
                city.ThumbnailImageId,
                city.ThumbnailImage != null ? city.ThumbnailImage.Url : ""
            )
        );
        var cities= await _cityRepository.GetPaginatedListAsync(
            specification,
            selector,
            cancellationToken
        );
        
        return Result<PaginatedList<CityResponse>>.Success(cities);
    }
}
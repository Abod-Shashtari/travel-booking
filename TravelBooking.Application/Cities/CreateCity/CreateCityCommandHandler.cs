using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Cities.CreateCity;

public class CreateCityCommandHandler:IRequestHandler<CreateCityCommand,Result<CityResponse?>>
{
    private readonly IRepository<City> _cityRepository;
    private readonly IMapper _mapper;
    
    public CreateCityCommandHandler(IRepository<City> cityRepository, IMapper mapper)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
    }

    public async Task<Result<CityResponse?>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        var city = _mapper.Map<City>(request);
        if (await _cityRepository.IsExistAsync(city, cancellationToken))
            return Result<CityResponse>.Failure(CityErrors.CityAlreadyExists());
        await _cityRepository.AddAsync(city,cancellationToken);
        await _cityRepository.SaveChangesAsync(cancellationToken);
        var cityResponse = _mapper.Map<CityResponse>(city);
        return Result<CityResponse?>.Success(cityResponse);
    }
}
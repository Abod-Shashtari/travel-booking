using AutoMapper;
using MediatR;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Cities.UpdateCity;

public class UpdateCityCommandHandler:IRequestHandler<UpdateCityCommand,Result>
{
    private readonly IMapper _mapper;
    private readonly IRepository<City> _cityRepository;
    
    public UpdateCityCommandHandler(IMapper mapper, IRepository<City> cityRepository)
    {
        _mapper = mapper;
        _cityRepository = cityRepository;
    }
    public async Task<Result> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        var city = await _cityRepository.GetByIdAsync(request.CityId,cancellationToken);
        if(city==null) return Result.Failure(CityErrors.CityNotFound());
        
        _mapper.Map(request,city);
        await _cityRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
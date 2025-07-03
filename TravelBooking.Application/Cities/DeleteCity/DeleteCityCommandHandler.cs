using MediatR;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Cities.DeleteCity;

public class DeleteCityCommandHandler:IRequestHandler<DeleteCityCommand,Result>
{
    private readonly IRepository<City> _cityRepository;
    public DeleteCityCommandHandler(IRepository<City> cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<Result> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
    {
        var city= await _cityRepository.GetByIdAsync(request.CityId,cancellationToken);
        if (city == null) return Result.Failure(CityErrors.CityNotFound());
        
        _cityRepository.Delete(city);
        await _cityRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
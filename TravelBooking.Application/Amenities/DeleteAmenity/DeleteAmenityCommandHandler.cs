using MediatR;
using TravelBooking.Application.Cities.DeleteCity;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Amenities.Errors;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Amenities.DeleteAmenity;

public class DeleteAmenityCommandHandler:IRequestHandler<DeleteAmenityCommand,Result>
{
    private readonly IRepository<Amenity> _amenityRepository;
    public DeleteAmenityCommandHandler(IRepository<Amenity> amenityRepository)
    {
        _amenityRepository = amenityRepository;
    }

    public async Task<Result> Handle(DeleteAmenityCommand request, CancellationToken cancellationToken)
    {
        var amenity= await _amenityRepository.GetByIdAsync(request.AmenityId,cancellationToken);
        if (amenity == null) return Result.Failure(AmenityErrors.AmenityNotFound());
        
        _amenityRepository.Delete(amenity);
        await _amenityRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
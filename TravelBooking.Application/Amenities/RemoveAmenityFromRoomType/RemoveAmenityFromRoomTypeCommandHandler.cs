using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Amenities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace TravelBooking.Application.Amenities.RemoveAmenityFromRoomType;

public class RemoveAmenityFromRoomTypeCommandHandler:IRequestHandler<RemoveAmenityFromRoomTypeCommand, Result>
{
    private readonly IRepository<Amenity> _amenityRepository;
    private readonly IRepository<RoomType> _roomTypeRepository;
    
    public RemoveAmenityFromRoomTypeCommandHandler(IRepository<Amenity> amenityRepository, IRepository<RoomType> roomTypeRepository)
    {
        _amenityRepository = amenityRepository;
        _roomTypeRepository = roomTypeRepository;
    }
    
    public async Task<Result> Handle(RemoveAmenityFromRoomTypeCommand request, CancellationToken cancellationToken)
    {
        var amenity = await _amenityRepository.GetByIdAsync(request.AmenityId, cancellationToken);
        if (amenity==null)
            return Result.Failure(AmenityErrors.AmenityNotFound());
        
        var specification= new RoomTypesWithAmenitySpecification();
        var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId,specification, cancellationToken);
        if (roomType==null)
            return Result.Failure(RoomTypeErrors.RoomTypeNotFound());
        
        roomType.Amenities.Remove(amenity);
        await _roomTypeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
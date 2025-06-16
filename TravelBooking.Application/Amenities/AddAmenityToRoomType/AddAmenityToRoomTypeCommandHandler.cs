using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Amenities.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace TravelBooking.Application.Amenities.AddAmenityToRoomType;

public class AddAmenityToRoomTypeCommandHandler:IRequestHandler<AddAmenityToRoomTypeCommand,Result>
{
    private readonly IRepository<Amenity> _amenityRepository;
    private readonly IRepository<RoomType> _roomTypeRepository;
    
    public AddAmenityToRoomTypeCommandHandler(IRepository<Amenity> amenityRepository, IRepository<RoomType> roomTypeRepository)
    {
        _amenityRepository = amenityRepository;
        _roomTypeRepository = roomTypeRepository;
    }

    public async Task<Result> Handle(AddAmenityToRoomTypeCommand request, CancellationToken cancellationToken)
    {
        var amenity = await _amenityRepository.GetByIdAsync(request.AmenityId, cancellationToken);
        if (amenity==null)
            return Result<AmenityResponse>.Failure(AmenityErrors.AmenityNotFound());
        
        var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId, cancellationToken);
        if (roomType==null)
            return Result<RoomTypeResponse>.Failure(RoomTypeErrors.RoomTypeNotFound());
        
        roomType.Amenities.Add(amenity);
        await _roomTypeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
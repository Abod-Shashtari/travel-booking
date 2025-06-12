using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace TravelBooking.Application.RoomTypes.GetRoomType;

public class GetRoomTypeQueryHandler:IRequestHandler<GetRoomTypeQuery, Result<RoomTypeResponse?>>
{
    private readonly IRepository<RoomType> _roomTypeRepository;

    public GetRoomTypeQueryHandler(IRepository<RoomType> repository)
    {
        _roomTypeRepository = repository;
    }

    public async Task<Result<RoomTypeResponse?>> Handle(GetRoomTypeQuery request, CancellationToken cancellationToken)
    {
        var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId,cancellationToken);
        if (roomType == null) return Result<RoomTypeResponse?>.Failure(RoomTypeErrors.RoomTypeNotFound());

        var roomTypeResponse = new RoomTypeResponse(
            roomType.Id,
            roomType.HotelId,
            roomType.Name,
            roomType.Description,
            roomType.PricePerNight,
            roomType.PricePerNight,
            roomType.CreatedAt,
            roomType.ModifiedAt
        );
        
        return Result<RoomTypeResponse?>.Success(roomTypeResponse);
    }
}
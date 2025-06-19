using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Common.Models;

public class RoomsByRoomTypeIdSpecification:PaginationSpecification<Room>
{
    public RoomsByRoomTypeIdSpecification(Guid roomTypeId,int pageNumber,int pageSize) :
        base(pageNumber, pageSize,amenity=>amenity.CreatedAt,true)
    {
        Criteria = room => room.RoomTypeId==roomTypeId;
    }
}
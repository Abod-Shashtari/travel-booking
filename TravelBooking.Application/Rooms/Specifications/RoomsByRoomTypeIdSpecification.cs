using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Rooms.Specifications;

public class RoomsByRoomTypeIdSpecification:PaginationSpecification<Room>
{
    public RoomsByRoomTypeIdSpecification(Guid roomTypeId,int pageNumber,int pageSize) :
        base(pageNumber, pageSize,amenity=>amenity.CreatedAt,true)
    {
        Criteria = room => room.RoomTypeId==roomTypeId;
    }
}
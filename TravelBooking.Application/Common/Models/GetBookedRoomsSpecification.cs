using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Common.Models;

public class GetBookedRoomsSpecification:Specification<Room>
{
    public GetBookedRoomsSpecification(List<Guid> roomIds)
    {
        Criteria=r=>roomIds.Contains(r.Id);
        AddInclude(r=>r.RoomType!);
    }
}
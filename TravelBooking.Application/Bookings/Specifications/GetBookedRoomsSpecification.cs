using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Bookings.Specifications;

public class GetBookedRoomsSpecification:Specification<Room>
{
    public GetBookedRoomsSpecification(List<Guid> roomIds)
    {
        Criteria=r=>roomIds.Contains(r.Id);
        AddInclude(r=>r.RoomType!);
    }
}
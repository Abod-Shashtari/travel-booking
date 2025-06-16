using System.Linq.Expressions;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Common.Models;

public class RoomsByRoomTypeIdSpecification:ISpecification<Room>
{
    public Expression<Func<Room, bool>>? Criteria { get; }
    public List<Expression<Func<Room, object>>>? Includes { get; } = null;

    public RoomsByRoomTypeIdSpecification(Guid roomTypeId)
    {
        Criteria = room => room.RoomTypeId==roomTypeId;
    }
}
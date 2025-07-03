using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.Bookings.Entities;

namespace TravelBooking.Application.Bookings.Specifications;

public class IncludeNavigationsForABookSpecification:Specification<Booking>
{
    public IncludeNavigationsForABookSpecification()
    {
        AddInclude(b=>b.Rooms);
        AddInclude("Rooms.RoomType");
        AddInclude(b=>b.User!);
        AddInclude(b=>b.Hotel!);
    }
}
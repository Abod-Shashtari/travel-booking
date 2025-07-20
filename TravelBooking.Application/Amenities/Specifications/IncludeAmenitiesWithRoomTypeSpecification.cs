using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Application.Amenities.Specifications;

public class IncludeAmenitiesWithRoomTypeSpecification:Specification<RoomType>
{
    public IncludeAmenitiesWithRoomTypeSpecification()
    {
        AddInclude(rt=>rt.Amenities);
    }
}
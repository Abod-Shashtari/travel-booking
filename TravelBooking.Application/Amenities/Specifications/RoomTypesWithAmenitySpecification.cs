using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Application.Amenities.Specifications;

public class RoomTypesWithAmenitySpecification:Specification<RoomType>
{
    public RoomTypesWithAmenitySpecification()
    {
        AddInclude(rt=>rt.Amenities);
    }
}
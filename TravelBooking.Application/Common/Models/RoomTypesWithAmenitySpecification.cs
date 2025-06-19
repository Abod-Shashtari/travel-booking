using System.Linq.Expressions;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Application.Common.Models;

public class RoomTypesWithAmenitySpecification:Specification<RoomType>
{
    public RoomTypesWithAmenitySpecification()
    {
        AddInclude(rt=>rt.Amenities);
    }
}
using System.Linq.Expressions;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Application.Common.Models;

public class RoomTypesWithAmenitySpecification:ISpecification<RoomType>
{
    public Expression<Func<RoomType, bool>>? Criteria { get; } = null;
    public List<Expression<Func<RoomType, object>>>? Includes { get; }

    public RoomTypesWithAmenitySpecification()
    {
        Includes = [rt=>rt.Amenities];
    }
}
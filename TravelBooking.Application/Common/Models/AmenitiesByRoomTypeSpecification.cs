using System.Linq.Expressions;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Common.Models;

public class AmenitiesByRoomTypeSpecification:ISpecification<Amenity>
{
    public Expression<Func<Amenity, bool>>? Criteria { get; }
    public List<Expression<Func<Amenity, object>>>? Includes { get; } = null;

    public AmenitiesByRoomTypeSpecification(Guid roomTypeId)
    {
        Criteria = a => a.RoomsTypes.Any(rt=>rt.Id==roomTypeId);
    }
}
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.Amenities.Entities;

namespace TravelBooking.Application.Amenities.Specifications;

public class AmenitiesByRoomTypeSpecification:PaginationSpecification<Amenity>
{
    public AmenitiesByRoomTypeSpecification(Guid roomTypeId,int pageNumber,int pageSize) :
        base(pageNumber, pageSize,amenity=>amenity.CreatedAt,true)
    {
        Criteria = a => a.RoomsTypes.Any(rt=>rt.Id==roomTypeId);
    }
}
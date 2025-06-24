using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Common.Models;

public class GetFeaturedHotelsSpecification:PaginationSpecification<Hotel>
{
    public GetFeaturedHotelsSpecification(int numberOfHotels):base(1,numberOfHotels)
    {
        Criteria = h =>
            h.RoomTypes.Any(rt => 
                rt.Discounts.Any(
                    d=>d.EndDate > DateTime.UtcNow && d.StartDate <= DateTime.UtcNow
                )
            );
    }
}
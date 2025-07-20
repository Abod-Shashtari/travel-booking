using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.UserActivity.Entites;

namespace TravelBooking.Application.Hotels.Specifications;

public class HotelVisitByUserAndHotelSpecification:Specification<HotelVisit>
{
    public HotelVisitByUserAndHotelSpecification(Guid hotelId,Guid visitorUserId)
    {
        Criteria=hv=>hv.HotelId==hotelId && hv.UserId==visitorUserId;
    }
}
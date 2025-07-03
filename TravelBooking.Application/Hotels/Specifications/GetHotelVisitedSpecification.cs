using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.UserActivity.Entites;

namespace TravelBooking.Application.Hotels.Specifications;

public class GetHotelVisitedSpecification:Specification<HotelVisit>
{
    public GetHotelVisitedSpecification(Guid hotelId,Guid visitorUserId)
    {
        Criteria=hv=>hv.HotelId==hotelId && hv.UserId==visitorUserId;
    }
}
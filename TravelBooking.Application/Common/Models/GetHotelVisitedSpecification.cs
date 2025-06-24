using TravelBooking.Domain.UserActivity.Entites;

namespace TravelBooking.Application.Common.Models;

public class GetHotelVisitedSpecification:Specification<HotelVisit>
{
    public GetHotelVisitedSpecification(Guid hotelId,Guid visitorUserId)
    {
        Criteria=hv=>hv.HotelId==hotelId && hv.UserId==visitorUserId;
    }
}
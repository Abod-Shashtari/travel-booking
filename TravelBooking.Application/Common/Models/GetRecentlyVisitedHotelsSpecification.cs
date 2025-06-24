using TravelBooking.Domain.UserActivity.Entites;

namespace TravelBooking.Application.Common.Models;

public class GetRecentlyVisitedHotelsSpecification:PaginationSpecification<HotelVisit>
{

    public GetRecentlyVisitedHotelsSpecification(Guid userId, int numberOfHotels)
        : base(1, numberOfHotels, hv=>hv.CreatedAt, true)
    {
        Criteria=hv=>hv.UserId == userId;
        AddInclude(hv=>hv.Hotel!);
    }
}
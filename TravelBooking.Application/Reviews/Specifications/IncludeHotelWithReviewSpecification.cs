using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.Reviews.Entities;

namespace TravelBooking.Application.Reviews.Specifications;

public class IncludeHotelWithReviewSpecification:Specification<Review>
{
    public IncludeHotelWithReviewSpecification()
    {
        AddInclude(r=>r.Hotel!);
    }
}
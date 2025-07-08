using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.Reviews.Entities;

namespace TravelBooking.Application.Reviews.Specifications;

public class GetReviewsOfHotelSpecification:Specification<Review>
{
    public GetReviewsOfHotelSpecification(Guid hotelId,int pageNumber, int pageSize)
    {
        Criteria = r=> r.HotelId==hotelId;
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderByDescending(rt=>rt.CreatedAt);
    }
}
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Application.RoomTypes.Specifications;

public class RoomTypeWithDiscountsSpecification:Specification<RoomType>
{
    public RoomTypeWithDiscountsSpecification()
    {
        AddInclude(rt=>rt.Discounts);
    }
    public RoomTypeWithDiscountsSpecification(int pageNumber, int pageSize)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderByDescending(rt=>rt.CreatedAt);
    }
}
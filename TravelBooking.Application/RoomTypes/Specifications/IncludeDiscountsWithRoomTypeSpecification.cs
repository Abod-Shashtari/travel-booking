using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Application.RoomTypes.Specifications;

public class IncludeDiscountsWithRoomTypeSpecification:Specification<RoomType>
{
    public IncludeDiscountsWithRoomTypeSpecification()
    {
        AddInclude(rt=>rt.Discounts);
    }
    public IncludeDiscountsWithRoomTypeSpecification(int pageNumber, int pageSize)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderByDescending(rt=>rt.CreatedAt);
    }
}
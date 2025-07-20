using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Application.RoomTypes.Specifications;

public class HotelRoomTypesIncludingDiscountsSpecification:Specification<RoomType>
{
    public HotelRoomTypesIncludingDiscountsSpecification(Guid hotelId, int pageNumber, int pageSize)
    {
        Criteria = rt => rt.HotelId==hotelId;
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderByDescending(rt=>rt.CreatedAt);
        AddInclude(rt=>rt.Discounts);
    }
}
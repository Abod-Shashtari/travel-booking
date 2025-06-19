using System.Linq.Expressions;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Application.Common.Models;

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
using System.Linq.Expressions;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Application.Common.Models;

public class RoomTypeWithDiscountsSpecification:ISpecification<RoomType>
{
    public Expression<Func<RoomType, bool>>? Criteria => null;
    public List<Expression<Func<RoomType, object>>>? Includes { get; }

    public RoomTypeWithDiscountsSpecification()
    {
        Includes = [rt => rt.Discounts];
    }
}
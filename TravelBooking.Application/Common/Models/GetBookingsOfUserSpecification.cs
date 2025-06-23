using System.Linq.Expressions;
using TravelBooking.Domain.Bookings.Entities;

namespace TravelBooking.Application.Common.Models;

public class GetBookingsOfUserSpecification:PaginationSpecification<Booking>
{

    public GetBookingsOfUserSpecification(Guid userId, int pageNumber, int pageSize, Expression<Func<Booking, object>> orderBy, bool descending) : base(pageNumber, pageSize, orderBy, descending)
    {
        Criteria = b => b.UserId == userId;
        AddInclude(b=>b.Rooms);
    }
}
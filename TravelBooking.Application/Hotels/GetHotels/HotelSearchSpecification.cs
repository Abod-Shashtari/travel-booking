using System.Linq.Expressions;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.GetHotels;

public class HotelSearchSpecification:ISpecification<Hotel>
{
    public Expression<Func<Hotel, bool>>? Criteria { get; }
    public List<Expression<Func<Hotel, object>>>? Includes { get; } = null;

    public HotelSearchSpecification(HotelFilter? hotelFilter)
    {
        if (hotelFilter == null) return;
        
        Criteria = hotel =>
            (string.IsNullOrEmpty(hotelFilter.Keyword) ||
             hotel.Name.Contains(hotelFilter.Keyword) ||
             hotel.City!.Name.Contains(hotelFilter.Keyword));
    }

}
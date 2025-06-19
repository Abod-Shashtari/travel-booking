using System.Linq.Expressions;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.GetHotels;

public class HotelSearchSpecification:PaginationSpecification<Hotel>
{
    public HotelSearchSpecification(HotelFilter? hotelFilter,int pageNumber,int pageSize) :
        base(pageNumber, pageSize,hotel=>hotel.CreatedAt,true)
    {
        if (hotelFilter == null) return;
        
        Criteria = hotel =>
            (string.IsNullOrEmpty(hotelFilter.Keyword) ||
             hotel.Name.Contains(hotelFilter.Keyword) ||
             hotel.City!.Name.Contains(hotelFilter.Keyword));
    }

}
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Application.Hotels.GetHotels;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.Hotels.Specifications;

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
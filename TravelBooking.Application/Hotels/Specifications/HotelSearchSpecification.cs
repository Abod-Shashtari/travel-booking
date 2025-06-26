using TravelBooking.Application.Common.Specifications;
using TravelBooking.Application.Hotels.GetHotels;
using TravelBooking.Domain.Bookings.Entities;
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
             hotel.City!.Name.Contains(hotelFilter.Keyword)) &&

            (hotelFilter.StarRating == null || hotel.StarRating >= hotelFilter.StarRating) &&

            (hotelFilter.RoomTypes == null || !hotelFilter.RoomTypes.Any() ||
             hotel.RoomTypes.Any(rt => hotelFilter.RoomTypes.Contains(rt.Name))) &&

            (hotelFilter.Amenities == null || !hotelFilter.Amenities.Any() ||
             hotelFilter.Amenities.All(a => 
                 hotel.RoomTypes.Any(rt => rt.Amenities.Any(am => am.Name == a)))) &&

            hotel.RoomTypes.SelectMany(rt => rt.Rooms).Count(room =>
                room.AdultCapacity >= (hotelFilter.NumberOfAdults ?? 0) &&
                room.ChildrenCapacity >= (hotelFilter.NumberOfChildren ?? 0) &&
                
                (hotelFilter.CheckIn == null || hotelFilter.CheckOut == null ||
                 !room.Bookings.Any(booking =>
                     booking.Status != BookingStatus.Cancelled &&
                     booking.Status != BookingStatus.Completed &&
                     booking.CheckIn < hotelFilter.CheckOut &&
                     booking.CheckOut > hotelFilter.CheckIn)) &&
                
                (hotelFilter.MinPrice == null || room.RoomType!.PricePerNight >= hotelFilter.MinPrice) &&
                (hotelFilter.MaxPrice == null || room.RoomType!.PricePerNight <= hotelFilter.MaxPrice)
            ) >= (hotelFilter.NumberOfRooms ?? 1);
    }
}
using AutoMapper;
using TravelBooking.Application.Bookings.CreateBooking;
using TravelBooking.Application.Bookings.GetBookings;
using TravelBooking.Web.Requests.Bookings;

namespace TravelBooking.Web.Profiles;

public class BookingRequestProfile:Profile
{
    public BookingRequestProfile()
    {
        CreateMap<CreateBookingRequest, CreateBookingCommand>()
            .ForCtorParam(
                "UserId", 
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            );
        CreateMap<GetBookingsRequest, GetBookingsQuery>()
            .ForCtorParam(
                "UserId", 
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            );
    }
}

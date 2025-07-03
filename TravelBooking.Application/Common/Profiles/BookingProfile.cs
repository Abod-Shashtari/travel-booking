using AutoMapper;
using TravelBooking.Application.Bookings.CreateBooking;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Bookings.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class BookingProfile:Profile
{
    public BookingProfile()
    {
        CreateMap<CreateBookingCommand, Booking>()
            .ForMember(dest => dest.Rooms, opt => opt.Ignore());
        
        CreateMap<Booking, BookingResponse>()
            .ForCtorParam(
                "RoomIds", 
                opt => opt.MapFrom((src, ctx) => src.Rooms.Select(r=>r.Id).ToList())
            )
            .ForCtorParam(
                "Status", 
                opt => opt.MapFrom((src, ctx) => src.Status.ToString()) 
            );
    }
}
using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IRepository<Booking>>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class BookingRepository:Repository<Booking>
{
    private readonly TravelBookingDbContext _context;
    
    public BookingRepository(TravelBookingDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<bool> IsExistAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        var roomIds = booking.Rooms.Select(r=>r.Id).ToList();
        
        return await _context.Bookings.AnyAsync(b=>
                b.CheckIn <= booking.CheckIn && 
                b.CheckOut >= booking.CheckOut &&
                b.Rooms.Any(room => roomIds.Contains(room.Id)) &&
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.Completed
            ,cancellationToken
        );
    }
}
using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IRepository<Hotel>>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class HotelRepository:Repository<Hotel>
{
    private TravelBookingDbContext _context;

    public HotelRepository(TravelBookingDbContext context):base(context)
    {
        _context = context;
    }

    public override async Task<bool> IsExistAsync(Hotel hotel, CancellationToken cancellationToken = default)
    {
        return await _context.Hotels.AnyAsync(h=>
                h.Name==hotel.Name && 
                (h.CityId==hotel.CityId || h.Location==hotel.Location),
            cancellationToken
        );
    }
}
using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IRepository<Room>>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class RoomRepository:Repository<Room>
{
    private readonly TravelBookingDbContext _context;
    
    public RoomRepository(TravelBookingDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<bool> IsExistAsync(Room room, CancellationToken cancellationToken = default)
    {
        return await _context.Rooms.AnyAsync(r =>
                r.RoomTypeId == room.RoomTypeId &&
                r.Number == room.Number,
            cancellationToken
        );
    }
}
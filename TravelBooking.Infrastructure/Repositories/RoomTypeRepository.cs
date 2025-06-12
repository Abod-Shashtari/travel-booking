using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IRepository<RoomType>>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class RoomTypeRepository:Repository<RoomType>
{
    private readonly TravelBookingDbContext _context;
    public RoomTypeRepository(TravelBookingDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<bool> IsExistAsync(RoomType roomType, CancellationToken cancellationToken = default)
    {
        return await _context.RoomsTypes.AnyAsync(
            rt=>rt.HotelId == roomType.HotelId && rt.Name==roomType.Name,
            cancellationToken
        );
    }
}
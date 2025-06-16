using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IRepository<Amenity>>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class AmenityRepository:Repository<Amenity>
{
    private readonly TravelBookingDbContext _context;
    
    public AmenityRepository(TravelBookingDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<bool> IsExistAsync(Amenity amenity, CancellationToken cancellationToken = default)
    {
        return await _context.Amenities.AnyAsync(a=>a.Name == amenity.Name, cancellationToken);
    }
}
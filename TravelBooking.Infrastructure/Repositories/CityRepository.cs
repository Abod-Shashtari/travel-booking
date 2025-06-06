using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IRepository<City>>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class CityRepository:Repository<City>
{
    private readonly TravelBookingDbContext _context;
    
    public CityRepository(TravelBookingDbContext context):base(context)
    {
        _context = context;
    }

    public override async Task<bool> IsExistAsync(City city, CancellationToken cancellationToken = default)
    {
        return await _context.Cities.AnyAsync(c=>c.Name == city.Name && c.Country==city.Country, cancellationToken);
    }
}
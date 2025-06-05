using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Infrastructure;

[ServiceImplementation]
[RegisterAs<IRepository<City>>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class CityRepository:IRepository<City>
{
    private readonly TravelBookingDbContext _context;
    
    public CityRepository(TravelBookingDbContext context)
    {
        _context = context;
    }

    public Task<City?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedList<City>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> AddAsync(City city, CancellationToken cancellationToken = default)
    {
        await _context.Cities.AddAsync(city, cancellationToken);
        return city.Id;
    }

    public async Task<bool> IsExistAsync(City city, CancellationToken cancellationToken = default)
    {
        return await _context.Cities.AnyAsync(c=>c.Name == city.Name && c.Country==city.Country, cancellationToken);
    }

    public void Delete(City city)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
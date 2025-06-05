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
public class HotelRepository:IRepository<Hotel>
{
    private TravelBookingDbContext _context;

    public HotelRepository(TravelBookingDbContext context)
    {
        _context = context;
    }

    public Task<Hotel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedList<Hotel>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> AddAsync(Hotel hotel, CancellationToken cancellationToken = default)
    {
        await _context.Hotels.AddAsync(hotel, cancellationToken);
        return hotel.Id;
    }

    public async Task<bool> IsExistAsync(Hotel hotel, CancellationToken cancellationToken = default)
    {
        return await _context.Hotels.AnyAsync(h=>
                h.Name==hotel.Name && 
                (h.CityId==hotel.CityId || h.Location==hotel.Location),
            cancellationToken
        );
    }

    public void Delete(Hotel hotel)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
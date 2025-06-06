using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Infrastructure.Repositories;

public abstract class Repository<T>:IRepository<T> where T:EntityBase
{
    private readonly DbSet<T> _dbSet;
    private readonly TravelBookingDbContext _context;

    protected Repository(TravelBookingDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<PaginatedList<T>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await _context.Cities.CountAsync(cancellationToken);
        var data= await _dbSet.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PaginatedList<T>(data,totalCount,pageSize,pageNumber);
    }

    public async Task<Guid> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity.Id;
    }

    public abstract Task<bool> IsExistAsync(T entity, CancellationToken cancellationToken = default);

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
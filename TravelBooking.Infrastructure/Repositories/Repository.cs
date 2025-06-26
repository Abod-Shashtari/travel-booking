using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

    public async Task<T?> GetByIdAsync(Guid id, ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        query = query.Where(specification.Criteria);
        foreach (var include in specification.Includes)
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(e=>e.Id==id,cancellationToken);
    }

    public async Task<TResult?> GetByIdAsync<TResult>(Guid id, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(entity=>entity.Id==id).Select(selector).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PaginatedList<T>> GetPaginatedListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var (query, totalCount) = await ApplySpecificationForPages(specification, cancellationToken);
        
        var data = await query.ToListAsync(cancellationToken);
        
        if(specification.Skip==null || specification.Take==null)
            return new PaginatedList<T>(data,totalCount, data.Count , 1);
        
        return new PaginatedList<T>(data,totalCount, (int)specification.Take!, (int)specification.Skip! / (int)specification.Take! + 1);
    }

    public async Task<PaginatedList<TResult>> GetPaginatedListAsync<TResult>(ISpecification<T> specification, Expression<Func<T, TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        var (query, totalCount) = await ApplySpecificationForPages(specification, cancellationToken);
        
        var data = await query.Select(selector).ToListAsync(cancellationToken);
        
        if(specification.Skip==null || specification.Take==null)
            return new PaginatedList<TResult>(data,totalCount, data.Count , 1);
        
        return new PaginatedList<TResult>(data,totalCount, (int)specification.Take!, (int)specification.Skip! / (int)specification.Take! + 1);
    }

    private async Task<(IQueryable<T>, int)> ApplySpecificationForPages(ISpecification<T> specification,CancellationToken cancellationToken)
    {
        IQueryable<T> query = _dbSet;
        
        query = query.Where(specification.Criteria);
        foreach (var include in specification.Includes)
        {
            query = query.Include(include);
        }
        
        if (specification.OrderBy != null)
            query = query.OrderBy(specification.OrderBy);
        else if (specification.OrderByDescending != null)
            query = query.OrderByDescending(specification.OrderByDescending);

        var totalCount = await query.CountAsync(cancellationToken);
        
        if (specification.Skip != null)
            query = query.Skip((int)specification.Skip);
        if (specification.Take != null)
            query = query.Take((int)specification.Take);
        
        return (query,totalCount);
    }

    public async Task<Guid> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity.Id;
    }

    public abstract Task<bool> IsExistAsync(T entity, CancellationToken cancellationToken = default);
    public async Task<bool> IsExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
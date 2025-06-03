using TravelBooking.Domain.Common.Entities;

namespace TravelBooking.Domain.Common.Interfaces;

public interface IRepository<T> where T : EntityBase
{
    Task<T?> GetByIdAsync(Guid id,CancellationToken cancellationToken = default);
    Task<PaginatedList<T>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(T entity, CancellationToken cancellationToken = default);
    void Delete(T entity);
    Task <int> SaveChangesAsync(CancellationToken cancellationToken = default); 
}
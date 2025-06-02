using TravelBooking.Domain.Common.Entities;

namespace TravelBooking.Domain.Common.Interfaces;

public interface IRepository<T> where T : EntityBase
{
    Task<T?> GetByIdAsync(Guid id);
    Task<PaginatedList<T>> GetAllAsync(int pageNumber, int pageSize);
    Task<Guid> AddAsync(T entity);
    void Delete(T entity);
    Task <int> SaveChangesAsync(); 
}
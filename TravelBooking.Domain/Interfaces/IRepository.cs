using TravelBooking.Domain.Entities;
using TravelBooking.Domain.Shared;

namespace TravelBooking.Domain.Interfaces;

public interface IRepository<T> where T : EntityBase
{
    Task<T?> GetByIdAsync(Guid id);
    Task<PaginatedList<T>> GetAllAsync(int pageNumber, int pageSize);
    Task<Guid> AddAsync(T entity);
    void Delete(T entity);
    Task <int> SaveChangesAsync(); 
}
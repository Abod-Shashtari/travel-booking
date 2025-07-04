﻿using System.Linq.Expressions;
using TravelBooking.Domain.Common.Entities;

namespace TravelBooking.Domain.Common.Interfaces;

public interface IRepository<T> where T : EntityBase
{
    Task<T?> GetByIdAsync(Guid id,CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id,ISpecification<T> specification,CancellationToken cancellationToken = default);
    Task<TResult?> GetByIdAsync<TResult>(Guid id,Expression<Func<T, TResult>> selector,CancellationToken cancellationToken = default);
    Task<PaginatedList<T>> GetPaginatedListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<PaginatedList<TResult>> GetPaginatedListAsync<TResult>(ISpecification<T> specification,Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> IsExistAsync(T entity,CancellationToken cancellationToken = default);
    Task<bool> IsExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Delete(T entity);
    Task <int> SaveChangesAsync(CancellationToken cancellationToken = default); 
}
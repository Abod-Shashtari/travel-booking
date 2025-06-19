using System.Linq.Expressions;
using TravelBooking.Domain.Common.Entities;

namespace TravelBooking.Application.Common.Models;

public class PaginationSpecification<T>:Specification<T> where T : EntityBase
{
    public PaginationSpecification(int pageNumber, int pageSize)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }

    public PaginationSpecification(int pageNumber, int pageSize, Expression<Func<T, object>> orderBy, bool descending)
        : this(pageNumber, pageSize)
    {
        if (descending)
            ApplyOrderByDescending(orderBy);
        else
            ApplyOrderBy(orderBy);
    }
}
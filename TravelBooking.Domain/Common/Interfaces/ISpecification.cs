using System.Linq.Expressions;

namespace TravelBooking.Domain.Common.Interfaces;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
}
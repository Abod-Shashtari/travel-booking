using TravelBooking.Domain.Entities;

namespace TravelBooking.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
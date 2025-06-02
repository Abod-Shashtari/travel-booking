using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Domain.Users.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
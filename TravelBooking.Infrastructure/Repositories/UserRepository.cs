using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Interfaces;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IUserRepository>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class UserRepository:IUserRepository
{
    private readonly TravelBookingDbContext _context;
    public UserRepository(TravelBookingDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.FindAsync<User>(id);
    }

    public Task<PaginatedList<User>> GetAllAsync(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        return user.Id;
    }

    public void Delete(User entity)
    {
        _context.Users.Remove(entity);
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
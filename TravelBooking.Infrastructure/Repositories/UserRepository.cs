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

    public async Task<User?> GetByIdAsync(Guid id,CancellationToken cancellationToken=default)
    {
        return await _context.Users.FindAsync(new object[] { id }, cancellationToken);
    }

    public Task<PaginatedList<User>> GetAllAsync(int pageNumber, int pageSize,CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> AddAsync(User user,CancellationToken cancellationToken=default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        return user.Id;
    }

    public void Delete(User entity)
    {
        _context.Users.Remove(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken=default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email,CancellationToken cancellationToken=default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Interfaces;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<IUserRepository>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class UserRepository:Repository<User>, IUserRepository
{
    private readonly TravelBookingDbContext _context;
    public UserRepository(TravelBookingDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<bool> IsExistAsync(User user, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == user.Email, cancellationToken);
    }
    public async Task<User?> GetByEmailAsync(string email,CancellationToken cancellationToken=default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
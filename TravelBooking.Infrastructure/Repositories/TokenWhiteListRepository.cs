using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Authentication.Entities;
using TravelBooking.Domain.Authentication.Interfaces;
using TravelBooking.Domain.Users.Interfaces;

namespace TravelBooking.Infrastructure.Repositories;

[ServiceImplementation]
[RegisterAs<ITokenWhiteListRepository>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class TokenWhiteListRepository:ITokenWhiteListRepository
{
    private readonly TravelBookingDbContext _context;
    
    public TokenWhiteListRepository(TravelBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(TokenWhiteList entity)
    {
        await _context.TokenWhiteList.AddAsync(entity);
        return entity.Id;
    }

    public async Task<bool> IsTokenActiveAsync(string jti)
    {
        var tokenWhiteList=await _context.TokenWhiteList.FirstOrDefaultAsync(t => t.Jti== jti);
        return (tokenWhiteList!=null) && (tokenWhiteList.IsActive);
    }

    public async Task DeactivateTokenAsync(string jti)
    {
        var tokenWhiteList=await _context.TokenWhiteList.FirstOrDefaultAsync(t => t.Jti== jti);
        if(tokenWhiteList!=null) tokenWhiteList.IsActive = false;
    }

    public async Task DeactivateTokensByUserIdAsync(Guid userId)
    {
        var userTokens = await _context.TokenWhiteList.Where(t => t.UserId == userId).ToListAsync();
        foreach (var userToken in userTokens) userToken.IsActive = false;
    }

    public async Task<int> CleanTokenWhiteList()
    {
        var expiredTokenWhiteList = await _context.TokenWhiteList.Where(
            t => !t.IsActive || t.ExpiresAt < DateTime.UtcNow
        ).ToListAsync();
        
        _context.TokenWhiteList.RemoveRange(expiredTokenWhiteList);
        return expiredTokenWhiteList.Count;
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
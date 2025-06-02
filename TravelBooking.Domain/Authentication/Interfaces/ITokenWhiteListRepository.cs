using TravelBooking.Domain.Authentication.Entities;

namespace TravelBooking.Domain.Authentication.Interfaces;

public interface ITokenWhiteListRepository
{
    Task<Guid> AddAsync(TokenWhiteList entity);
    Task<bool> IsTokenActiveAsync(string token);
    Task DeactivateTokenAsync(string token);
    Task DeactivateTokensByUserIdAsync (Guid userId);
    Task<int> CleanTokenWhiteList();
    Task <int> SaveChangesAsync(); 
}
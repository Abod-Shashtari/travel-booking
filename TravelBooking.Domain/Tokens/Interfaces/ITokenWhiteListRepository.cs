using TravelBooking.Domain.Tokens.Entities;

namespace TravelBooking.Domain.Tokens.Interfaces;

public interface ITokenWhiteListRepository
{
    Task<Guid> AddAsync(TokenWhiteList entity,CancellationToken cancellationToken=default);
    Task<bool> IsTokenActiveAsync(string token, CancellationToken cancellationToken=default);
    Task DeactivateTokenAsync(string token, CancellationToken cancellationToken=default);
    Task DeactivateTokensByUserIdAsync (Guid userId,CancellationToken cancellationToken=default);
    Task<int> CleanTokenWhiteList(CancellationToken cancellationToken=default);
    Task <int> SaveChangesAsync(CancellationToken cancellationToken=default); 
}
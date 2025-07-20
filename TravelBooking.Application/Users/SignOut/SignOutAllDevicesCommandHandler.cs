using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Tokens.Interfaces;

namespace TravelBooking.Application.Users.SignOut;

public class SignOutAllDevicesCommandHandler:IRequestHandler<SignOutAllDevicesCommand,Result>
{
    private readonly ITokenWhiteListRepository _tokenWhiteListRepository;
    
    public SignOutAllDevicesCommandHandler(ITokenWhiteListRepository tokenWhiteListRepository)
    {
        _tokenWhiteListRepository = tokenWhiteListRepository;
    }
    public async Task<Result> Handle(SignOutAllDevicesCommand request, CancellationToken cancellationToken)
    {
        await _tokenWhiteListRepository.DeactivateTokensByUserIdAsync(request.UserId,cancellationToken);
        await _tokenWhiteListRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
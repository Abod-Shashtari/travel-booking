using MediatR;
using TravelBooking.Domain.Authentication.Interfaces;
using TravelBooking.Domain.Common;

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
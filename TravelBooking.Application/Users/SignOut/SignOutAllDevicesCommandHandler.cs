using MediatR;
using TravelBooking.Domain.Authentication.Interfaces;

namespace TravelBooking.Application.Users.SignOut;

public class SignOutAllDevicesCommandHandler:IRequestHandler<SignOutAllDevicesCommand>
{
    private readonly ITokenWhiteListRepository _tokenWhiteListRepository;
    
    public SignOutAllDevicesCommandHandler(ITokenWhiteListRepository tokenWhiteListRepository)
    {
        _tokenWhiteListRepository = tokenWhiteListRepository;
    }
    public async Task Handle(SignOutAllDevicesCommand request, CancellationToken cancellationToken)
    {
        await _tokenWhiteListRepository.DeactivateTokensByUserIdAsync(request.UserId,cancellationToken);
        await _tokenWhiteListRepository.SaveChangesAsync(cancellationToken);
    }
}
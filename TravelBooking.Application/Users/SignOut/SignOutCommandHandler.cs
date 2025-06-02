using MediatR;
using TravelBooking.Domain.Authentication.Interfaces;

namespace TravelBooking.Application.Users.SignOut;

public class SignOutCommandHandler:IRequestHandler<SignOutCommand>
{
    private readonly ITokenWhiteListRepository _tokenWhiteListRepository;
    
    public SignOutCommandHandler(ITokenWhiteListRepository tokenWhiteListRepository)
    {
        _tokenWhiteListRepository = tokenWhiteListRepository;
    }

    public async Task Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        await _tokenWhiteListRepository.DeactivateTokenAsync(request.TokenJti);
        await _tokenWhiteListRepository.SaveChangesAsync();
    }
}
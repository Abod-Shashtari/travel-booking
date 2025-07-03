using MediatR;
using TravelBooking.Domain.Authentication.Interfaces;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Users.SignOut;

public class SignOutCommandHandler:IRequestHandler<SignOutCommand,Result>
{
    private readonly ITokenWhiteListRepository _tokenWhiteListRepository;
    
    public SignOutCommandHandler(ITokenWhiteListRepository tokenWhiteListRepository)
    {
        _tokenWhiteListRepository = tokenWhiteListRepository;
    }

    public async Task<Result> Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        await _tokenWhiteListRepository.DeactivateTokenAsync(request.TokenJti,cancellationToken);
        await _tokenWhiteListRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
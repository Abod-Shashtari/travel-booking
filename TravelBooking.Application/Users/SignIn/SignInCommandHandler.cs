using MediatR;
using Microsoft.AspNetCore.Identity;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Domain.Authentication.Entities;
using TravelBooking.Domain.Authentication.Interfaces;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Interfaces;

namespace TravelBooking.Application.Users.SignIn;

public class SignInCommandHandler:IRequestHandler<SignInCommand,string>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ITokenWhiteListRepository _tokenWhiteListRepository;
    
    public SignInCommandHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IJwtTokenGenerator jwtTokenGenerator, ITokenWhiteListRepository tokenWhiteListRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _tokenWhiteListRepository = tokenWhiteListRepository;
    }

    public async Task<string> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if(user == null) throw new Exception();
        var requestedHashedPassword = _passwordHasher.HashPassword(null, request.Password);
        if(user.HashedPassword != requestedHashedPassword) throw new Exception();
        
        var token = _jwtTokenGenerator.GenerateToken(user);
        await _tokenWhiteListRepository.AddAsync(new TokenWhiteList
        {
            UserId = user.Id,
            Jti = "",
            ExpiresAt = DateTime.Now.AddMinutes(5)
        });
        await _tokenWhiteListRepository.SaveChangesAsync();
        return token.Token;
    }
}
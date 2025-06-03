using System.Security.Authentication;
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
        const string invalidLoginMessage = "Invalid Email or Password";
        
        var user = await _userRepository.GetByEmailAsync(request.Email,cancellationToken);
        if(user == null) throw new InvalidCredentialException(invalidLoginMessage);
        
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed) throw new InvalidCredentialException(invalidLoginMessage);
        
        var token = _jwtTokenGenerator.GenerateToken(user);
        await _tokenWhiteListRepository.AddAsync(new TokenWhiteList
        {
            UserId = user.Id,
            Jti = token.Jti,
            ExpiresAt = token.ExpirationAt 
        },cancellationToken);
        await _tokenWhiteListRepository.SaveChangesAsync(cancellationToken);
        return token.Token;
    }
}
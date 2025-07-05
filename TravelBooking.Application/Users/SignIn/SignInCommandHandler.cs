using MediatR;
using Microsoft.AspNetCore.Identity;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Authentication.Entities;
using TravelBooking.Domain.Authentication.Interfaces;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Errors;
using TravelBooking.Domain.Users.Interfaces;

namespace TravelBooking.Application.Users.SignIn;

public class SignInCommandHandler:IRequestHandler<SignInCommand,Result<JwtTokenResponse?>>
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

    public async Task<Result<JwtTokenResponse?>> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email,cancellationToken);
        if (user == null) return Result<JwtTokenResponse?>.Failure(UserErrors.InvalidCredentialException());
        
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed) return Result<JwtTokenResponse?>.Failure(UserErrors.InvalidCredentialException());
        
        var token = _jwtTokenGenerator.GenerateToken(user);
        await _tokenWhiteListRepository.AddAsync(new TokenWhiteList
        {
            UserId = user.Id,
            Jti = token.Jti,
            ExpiresAt = token.ExpirationAt 
        },cancellationToken);
        var tokenResponse = new JwtTokenResponse(token.Token);
        await _tokenWhiteListRepository.SaveChangesAsync(cancellationToken);
        return Result<JwtTokenResponse?>.Success(tokenResponse);
    }
}
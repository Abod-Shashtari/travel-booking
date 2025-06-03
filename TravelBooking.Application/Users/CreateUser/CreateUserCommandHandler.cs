using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Errors;
using TravelBooking.Domain.Users.Exceptions;
using TravelBooking.Domain.Users.Interfaces;

namespace TravelBooking.Application.Users.CreateUser;

public class CreateUserCommandHandler:IRequestHandler<CreateUserCommand,Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IUserRepository userRepository ,IPasswordHasher<User> passwordHasher, IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userUseEmail = await _userRepository.GetByEmailAsync(request.Email,cancellationToken);
        if (userUseEmail != null) return Result<Guid>.Failure(UserErrors.EmailAlreadyUsed(request.Email));
        
        var hashedPassword= _passwordHasher.HashPassword(null, request.Password);
        var user = _mapper.Map<User>(request);
        user.HashedPassword = hashedPassword;
        
        var newId=await _userRepository.AddAsync(user,cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(newId);
    }
}
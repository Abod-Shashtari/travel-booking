using MediatR;
using Microsoft.AspNetCore.Identity;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Exceptions;
using TravelBooking.Domain.Users.Interfaces;

namespace TravelBooking.Application.Users.CreateUser;

public class CreateUserCommandHandler:IRequestHandler<CreateUserCommand,Guid>
{
    private IUserRepository _userRepository;
    private IPasswordHasher<User> _passwordHasher;

    public CreateUserCommandHandler(IUserRepository userRepository,IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userUseEmail = await _userRepository.GetByEmailAsync(request.Email);
        if (userUseEmail != null) throw new EmailAlreadyUsedException(request.Email);
        
        var hashedPassword= _passwordHasher.HashPassword(null, request.Password);
        var user = new User()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            HashedPassword = hashedPassword
        };
        
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return user.Id;
    }
}
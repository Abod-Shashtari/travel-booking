using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TravelBooking.Application.Users.CreateUser;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Errors;
using TravelBooking.Domain.Users.Interfaces;

namespace Application.Tests.Users;

public class CreateUserCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IPasswordHasher<User>> _passwordHasher;
    private readonly CreateUserCommandHandler _handler;
    private readonly Mock<IMapper> _mapper;

    public CreateUserCommandHandlerTests()
    {
        _fixture = new Fixture();
        _userRepository = new Mock<IUserRepository>();
        _passwordHasher = new Mock<IPasswordHasher<User>>();
        _mapper=new Mock<IMapper>();
        _handler = new CreateUserCommandHandler(_userRepository.Object, _passwordHasher.Object, _mapper.Object);
    }

    [Fact]
    public async Task CreateUserCommandHandler_ValidRequest_ShouldCreateUserAndReturnId()
    {
        // Arrange
        var command = _fixture.Create<CreateUserCommand>();
        var expectedUserId = _fixture.Create<Guid>();
        var hashedPassword = _fixture.Create<string>();
        var user = _fixture.Create<User>();

        _userRepository.Setup(x => x.GetByEmailAsync(command.Email,It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        _passwordHasher.Setup(x => x.HashPassword(null, command.Password))
            .Returns(hashedPassword);
        
        _mapper.Setup(x => x.Map<User>(command))
            .Returns(user);

        _userRepository.Setup(x => x.AddAsync(It.IsAny<User>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserId);

        _userRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedUserId);
        _userRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserCommandHandler_EmailAlreadyUsed_ShouldThrowEmailAlreadyUsedException()
    {
        // Arrange
        var command = _fixture.Create<CreateUserCommand>();
        var existingUser = new User { Email = command.Email };
        _userRepository.Setup(x => x.GetByEmailAsync(command.Email,It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.EmailAlreadyUsed(command.Email));
        _userRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void CreateUserCommand_NotValidEmail_ShouldReturnValidationResult()
    {
        // Arrange
        var command = _fixture.Build<CreateUserCommand>()
            .With(c => c.Email, "invalid")
            .Create();
        var validator = new CreateUserCommandValidator();
        // Act
        var result = validator.Validate(command);
        // Assert
        result.Errors.Should().Contain(v=>v.PropertyName==nameof(CreateUserCommand.Email));
    }
    
    [Fact]
    public void CreateUserCommand_NotValidPassword_ShouldReturnValidationResult()
    {
        // Arrange
        var command = _fixture.Build<CreateUserCommand>()
            .With(c => c.Password, "123")
            .Create();
        var validator = new CreateUserCommandValidator();
        // Act
        var result = validator.Validate(command);
        // Assert
        result.Errors.Should().Contain(v=>v.PropertyName==nameof(CreateUserCommand.Password));
    }
    [Fact]
    public void CreateUserCommand_PasswordsDoNotMatch_ShouldReturnValidationError()
    {
        // Arrange
        var command = _fixture.Build<CreateUserCommand>()
            .With(c => c.Password, "Pass123!")
            .With(c => c.ConfirmPassword, "Mismatch!")
            .Create();
        var validator = new CreateUserCommandValidator();
        // Act
        var result = validator.Validate(command);

        // Assert
        result.Errors.Should().Contain(v => v.PropertyName==nameof(CreateUserCommand.ConfirmPassword));
    }
}
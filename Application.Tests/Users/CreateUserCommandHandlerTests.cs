using System.ComponentModel.DataAnnotations;
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
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
    private readonly CreateUserCommandHandler _handler;
    private readonly Mock<IMapper> _mapperMock;

    public CreateUserCommandHandlerTests()
    {
        _fixture = new Fixture();
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();
        _mapperMock=new Mock<IMapper>();
        _handler = new CreateUserCommandHandler(_userRepositoryMock.Object, _passwordHasherMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateUserCommandHandler_ValidRequest_ShouldCreateUserAndReturnId()
    {
        var command = _fixture.Create<CreateUserCommand>();
        var expectedUserId = _fixture.Create<Guid>();
        var hashedPassword = _fixture.Create<string>();
        var user = _fixture.Create<User>();

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email,It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        _passwordHasherMock.Setup(x => x.HashPassword(null, command.Password))
            .Returns(hashedPassword);
        
        _mapperMock.Setup(x => x.Map<User>(command)) // Set up the mapping
            .Returns(user);

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserId);

        _userRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedUserId);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserCommandHandler_EmailAlreadyUsed_ShouldThrowEmailAlreadyUsedException()
    {
        // Arrange
        var command = _fixture.Create<CreateUserCommand>();
        var existingUser = new User { Email = command.Email };
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email,It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.EmailAlreadyUsed(command.Email));
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void CreateUserCommand_NotValidEmail_ShouldReturnValidationResult()
    {
        // Arrange
        var command = new CreateUserCommand { Email = "invalid" };
        // Act
        var result = Validate(command);
        // Assert
        result.Should().Contain(v=>v.MemberNames.Contains(nameof(CreateUserCommand.Password)));
    }
    
    [Fact]
    public void CreateUserCommand_NotValidPassword_ShouldReturnValidationResult()
    {
        // Arrange
        var command = new CreateUserCommand { Password = "123" };
        // Act
        var result = Validate(command);
        // Assert
        result.Should().Contain(v=>v.MemberNames.Contains(nameof(CreateUserCommand.Password)));
    }
    [Fact]
    public void CreateUserCommand_PasswordsDoNotMatch_ShouldReturnValidationError()
    {
        // Arrange
        var command = new CreateUserCommand { Password = "Pass123!", ConfirmPassword = "Mismatch!" };

        // Act
        var result = Validate(command);

        // Assert
        result.Should().Contain(v => v.MemberNames.Contains(nameof(CreateUserCommand.ConfirmPassword)));
    }

    private IList<ValidationResult> Validate(object obj)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(obj, serviceProvider: null, items: null);
        Validator.TryValidateObject(obj, context, results, validateAllProperties: true);
        return results;
    }
}
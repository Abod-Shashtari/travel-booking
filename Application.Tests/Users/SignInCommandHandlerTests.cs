using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Users.SignIn;
using TravelBooking.Domain.Authentication.Entities;
using TravelBooking.Domain.Authentication.Interfaces;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Errors;
using TravelBooking.Domain.Users.Interfaces;

namespace Application.Tests.Users;

public class SignInCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IPasswordHasher<User>> _passwordHasher;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGenerator;
    private readonly Mock<ITokenWhiteListRepository> _tokenWhiteListRepository;
    private readonly SignInCommandHandler _handler;

    public SignInCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _userRepository = new Mock<IUserRepository>();
        _passwordHasher = new Mock<IPasswordHasher<User>>();
        _jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        _tokenWhiteListRepository = new Mock<ITokenWhiteListRepository>();

        _handler = new SignInCommandHandler(
            _userRepository.Object,
            _passwordHasher.Object,
            _jwtTokenGenerator.Object,
            _tokenWhiteListRepository.Object);
    }

    [Fact]
    public async Task SignInCommandHandler_ValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var command = _fixture.Create<SignInCommand>();
        var user = _fixture.Build<User>()
            .With(u => u.Email, command.Email)
            .With(u => u.HashedPassword, "hashed_password")
            .Create();

        var jwtToken = new JwtTokenResult(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            DateTime.UtcNow.AddHours(1)
        );
        var tokenResponse = new JwtTokenResponse(jwtToken.Token);

        _userRepository.Setup(x => x.GetByEmailAsync(command.Email,It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(x => x.VerifyHashedPassword(user, user.HashedPassword, command.Password))
            .Returns(PasswordVerificationResult.Success);

        _jwtTokenGenerator.Setup(x => x.GenerateToken(user))
            .Returns(jwtToken);

        _tokenWhiteListRepository.Setup(x => x.AddAsync(It.IsAny<TokenWhiteList>(),It.IsAny<CancellationToken>()));

        _tokenWhiteListRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(tokenResponse);
        _tokenWhiteListRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SignInCommandHandler_UserNotFound_ShouldThrowInvalidCredentialException()
    {
        // Arrange
        var command = _fixture.Create<SignInCommand>();

        _userRepository.Setup(x => x.GetByEmailAsync(command.Email,It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentialException());
        _tokenWhiteListRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SignInCommandHandler_InvalidPassword_ShouldThrowInvalidCredentialException()
    {
        // Arrange
        var command = _fixture.Create<SignInCommand>();
        var user = _fixture.Build<User>()
            .With(u => u.Email, command.Email)
            .With(u => u.HashedPassword, "hashed_password")
            .Create();

        _userRepository.Setup(x => x.GetByEmailAsync(command.Email,It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasher.Setup(x =>
                x.VerifyHashedPassword(user, user.HashedPassword, command.Password))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentialException());
        _tokenWhiteListRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
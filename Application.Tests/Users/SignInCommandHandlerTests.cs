using System.Security.Authentication;
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
using TravelBooking.Domain.Users.Interfaces;

namespace Application.Tests.Users;

public class SignInCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly Mock<ITokenWhiteListRepository> _tokenWhiteListRepositoryMock;
    private readonly SignInCommandHandler _handler;

    public SignInCommandHandlerTests()
    {
        _fixture = new Fixture();
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _tokenWhiteListRepositoryMock = new Mock<ITokenWhiteListRepository>();

        _handler = new SignInCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenGeneratorMock.Object,
            _tokenWhiteListRepositoryMock.Object);
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

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email,It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(user, user.HashedPassword, command.Password))
            .Returns(PasswordVerificationResult.Success);

        _jwtTokenGeneratorMock.Setup(x => x.GenerateToken(user))
            .Returns(jwtToken);

        _tokenWhiteListRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TokenWhiteList>(),It.IsAny<CancellationToken>()));

        _tokenWhiteListRepositoryMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(jwtToken.Token);
        _tokenWhiteListRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SignInCommandHandler_UserNotFound_ShouldThrowInvalidCredentialException()
    {
        // Arrange
        var command = _fixture.Create<SignInCommand>();

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email,It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialException>();
        _tokenWhiteListRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
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

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email,It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(x =>
                x.VerifyHashedPassword(user, user.HashedPassword, command.Password))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialException>();
        _tokenWhiteListRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
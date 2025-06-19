using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Users.SignOut;
using TravelBooking.Domain.Authentication.Interfaces;

namespace Application.Tests.Users;

public class SignOutCommandHandlerTests
{
    private readonly Mock<ITokenWhiteListRepository> _tokenWhiteListRepository;
    private readonly SignOutAllDevicesCommandHandler _signOutAllDevicesHandler;
    private readonly SignOutCommandHandler _signOutHandler;
    private readonly IFixture _fixture;

    public SignOutCommandHandlerTests()
    {
        _tokenWhiteListRepository = new Mock<ITokenWhiteListRepository>();
        _signOutAllDevicesHandler = new SignOutAllDevicesCommandHandler(_tokenWhiteListRepository.Object);
        _signOutHandler = new SignOutCommandHandler(_tokenWhiteListRepository.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task SignOutAllDevicesCommandHandler_ValidRequest_ShouldDeactivateAllTokens()
    {
        // Arrange
        var command = _fixture.Create<SignOutAllDevicesCommand>();

        _tokenWhiteListRepository.Setup(r => r.DeactivateTokensByUserIdAsync(command.UserId,It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _tokenWhiteListRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _signOutAllDevicesHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _tokenWhiteListRepository.Verify(r => r.DeactivateTokensByUserIdAsync(command.UserId,It.IsAny<CancellationToken>()), Times.Once);
        _tokenWhiteListRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SignOutCommandHandler_ValidRequest_ShouldDeactivateSingleToken()
    {
        // Arrange
        var command = _fixture.Create<SignOutCommand>();

        _tokenWhiteListRepository.Setup(r => r.DeactivateTokenAsync(command.TokenJti,It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _tokenWhiteListRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _signOutHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _tokenWhiteListRepository.Verify(r => r.DeactivateTokenAsync(command.TokenJti,It.IsAny<CancellationToken>()), Times.Once);
        _tokenWhiteListRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
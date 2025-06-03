using AutoFixture;
using Moq;
using TravelBooking.Application.Users.SignOut;
using TravelBooking.Domain.Authentication.Interfaces;

namespace Application.Tests.Users;

public class SignOutCommandHandlerTests
{
    private readonly Mock<ITokenWhiteListRepository> _tokenWhiteListRepositoryMock;
    private readonly SignOutAllDevicesCommandHandler _signOutAllDevicesHandler;
    private readonly SignOutCommandHandler _signOutHandler;
    private readonly IFixture _fixture;

    public SignOutCommandHandlerTests()
    {
        _tokenWhiteListRepositoryMock = new Mock<ITokenWhiteListRepository>();
        _signOutAllDevicesHandler = new SignOutAllDevicesCommandHandler(_tokenWhiteListRepositoryMock.Object);
        _signOutHandler = new SignOutCommandHandler(_tokenWhiteListRepositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task SignOutAllDevicesCommandHandler_ValidRequest_ShouldDeactivateAllTokens()
    {
        // Arrange
        var command = _fixture.Create<SignOutAllDevicesCommand>();

        _tokenWhiteListRepositoryMock.Setup(r => r.DeactivateTokensByUserIdAsync(command.UserId))
            .Returns(Task.CompletedTask);

        _tokenWhiteListRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _signOutAllDevicesHandler.Handle(command, CancellationToken.None);

        // Assert
        _tokenWhiteListRepositoryMock.Verify(r => r.DeactivateTokensByUserIdAsync(command.UserId), Times.Once);
        _tokenWhiteListRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SignOutCommandHandler_ValidRequest_ShouldDeactivateSingleToken()
    {
        // Arrange
        var command = _fixture.Create<SignOutCommand>();

        _tokenWhiteListRepositoryMock.Setup(r => r.DeactivateTokenAsync(command.TokenJti))
            .Returns(Task.CompletedTask);

        _tokenWhiteListRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _signOutHandler.Handle(command, CancellationToken.None);

        // Assert
        _tokenWhiteListRepositoryMock.Verify(r => r.DeactivateTokenAsync(command.TokenJti), Times.Once);
        _tokenWhiteListRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
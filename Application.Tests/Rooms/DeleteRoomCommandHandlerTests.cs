using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Rooms.DeleteRoom;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Errors;

namespace Application.Tests.Rooms;

public class DeleteRoomCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Room>> _roomRepository;
    private readonly DeleteRoomCommandHandler _handler;

    public DeleteRoomCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _roomRepository = new Mock<IRepository<Room>>();
        _handler = new DeleteRoomCommandHandler(_roomRepository.Object);
    }

    [Fact]
    public async Task DeleteRoomCommandHandler_RoomExists_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<DeleteRoomCommand>();
        var room = _fixture.Create<Room>();

        _roomRepository.Setup(r => r.GetByIdAsync(command.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _roomRepository.Setup(r => r.Delete(room));
        _roomRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _roomRepository.Verify(r => r.Delete(It.IsAny<Room>()), Times.Once);
        _roomRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteRoomCommandHandler_RoomDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<DeleteRoomCommand>();

        _roomRepository.Setup(r => r.GetByIdAsync(command.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomErrors.RoomNotFound());
        _roomRepository.Verify(r => r.Delete(It.IsAny<Room>()), Times.Never);
        _roomRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Rooms.UpdateRoom;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Errors;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace Application.Tests.Rooms;

public class UpdateRoomCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Room>> _roomRepository;
    private readonly Mock<IRepository<RoomType>> _roomTypeRepository;
    private readonly UpdateRoomCommandHandler _handler;

    public UpdateRoomCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _roomRepository = new Mock<IRepository<Room>>();
        _roomTypeRepository = new Mock<IRepository<RoomType>>();
        var mapper = new Mock<IMapper>();

        _handler = new UpdateRoomCommandHandler(mapper.Object, _roomRepository.Object, _roomTypeRepository.Object);
    }

    [Fact]
    public async Task UpdateRoomCommandHandler_RoomTypeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<UpdateRoomCommand>();

        _roomTypeRepository.Setup(r => r.IsExistsByIdAsync(command.RoomTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomTypeErrors.RoomTypeAlreadyExists());
        _roomRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRoomCommandHandler_RoomDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<UpdateRoomCommand>();

        _roomTypeRepository.Setup(r => r.IsExistsByIdAsync(command.RoomTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _roomRepository.Setup(r => r.GetByIdAsync(command.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomErrors.RoomDoesNotExists());
        _roomRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRoomCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<UpdateRoomCommand>();
        var room = _fixture.Create<Room>();

        _roomTypeRepository.Setup(r => r.IsExistsByIdAsync(command.RoomTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _roomRepository.Setup(r => r.GetByIdAsync(command.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        _roomRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _roomRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
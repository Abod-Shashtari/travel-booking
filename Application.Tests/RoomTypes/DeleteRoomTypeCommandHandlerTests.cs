using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.RoomTypes.DeleteRoomType;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace Application.Tests.RoomTypes;

public class DeleteRoomTypeCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<RoomType>> _roomTypeRepository;
    private readonly DeleteRoomTypeCommandHandler _handler;

    public DeleteRoomTypeCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _roomTypeRepository = new Mock<IRepository<RoomType>>();
        _handler = new DeleteRoomTypeCommandHandler(_roomTypeRepository.Object);
    }

    [Fact]
    public async Task DeleteRoomTypeCommandHandler_NonExistingRoomType_ShouldReturnFailure()
    {
        // Arrange
        var roomTypeId = _fixture.Create<Guid>();
        var command = new DeleteRoomTypeCommand(roomTypeId);

        _roomTypeRepository
            .Setup(r => r.GetByIdAsync(roomTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoomType)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomTypeErrors.RoomTypeNotFound());
        _roomTypeRepository.Verify(r => r.Delete(It.IsAny<RoomType>()), Times.Never);
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteRoomTypeCommandHandler_ExistingRoomType_ShouldDeleteAndReturnSuccess()
    {
        // Arrange
        var roomTypeId = _fixture.Create<Guid>();
        var command = new DeleteRoomTypeCommand(roomTypeId);
        var roomType = _fixture.Create<RoomType>();

        _roomTypeRepository
            .Setup(r => r.GetByIdAsync(roomTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roomType);

        _roomTypeRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _roomTypeRepository.Verify(r => r.Delete(roomType), Times.Once);
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
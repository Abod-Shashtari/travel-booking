using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.RoomTypes.UpdateRoomType;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace Application.Tests.RoomTypes;

public class UpdateRoomTypeCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<RoomType>> _roomTypeRepository;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly UpdateRoomTypeCommandHandler _handler;

    public UpdateRoomTypeCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _roomTypeRepository = new Mock<IRepository<RoomType>>();
        _hotelRepository = new Mock<IRepository<Hotel>>();
        var mapper = new Mock<IMapper>();

        _handler = new UpdateRoomTypeCommandHandler(
            mapper.Object,
            _roomTypeRepository.Object,
            _hotelRepository.Object
        );
    }

    [Fact]
    public async Task UpdateRoomTypeCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<UpdateRoomTypeCommand>();
        var hotelId = command.HotelId;
        var roomTypeId = command.RoomTypeId;
        var existingRoomType = _fixture.Create<RoomType>();

        _hotelRepository.Setup(r => r.IsExistsByIdAsync(hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _roomTypeRepository.Setup(r => r.GetByIdAsync(roomTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRoomType);
        _roomTypeRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateRoomTypeCommandHandler_HotelDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<UpdateRoomTypeCommand>();
        _hotelRepository.Setup(r => r.IsExistsByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(HotelErrors.HotelNotFound());
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRoomTypeCommandHandler_RoomTypeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<UpdateRoomTypeCommand>();
        _hotelRepository.Setup(r => r.IsExistsByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _roomTypeRepository.Setup(r => r.GetByIdAsync(command.RoomTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoomType)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomTypeErrors.RoomTypeNotFound());
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
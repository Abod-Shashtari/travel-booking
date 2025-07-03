using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Rooms.CreateRoom;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Errors;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace Application.Tests.Rooms;

public class CreateRoomCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Room>> _roomRepository;
    private readonly Mock<IRepository<RoomType>> _roomTypeRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly CreateRoomCommandHandler _handler;

    public CreateRoomCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _roomRepository = new Mock<IRepository<Room>>();
        _roomTypeRepository = new Mock<IRepository<RoomType>>();
        _mapper = new Mock<IMapper>();

        _handler = new CreateRoomCommandHandler(
            _mapper.Object,
            _roomRepository.Object,
            _roomTypeRepository.Object
        );
    }

    [Fact]
    public async Task CreateRoomCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<CreateRoomCommand>();
        var room = _fixture.Create<Room>();
        var roomResponse = _fixture.Create<RoomResponse>();

        _roomTypeRepository.Setup(r => r.IsExistsByIdAsync(command.RoomTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mapper.Setup(m => m.Map<RoomResponse>(It.IsAny<Room>()))
            .Returns(roomResponse);
        _roomRepository.Setup(r => r.IsExistAsync(room, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _roomRepository.Setup(r => r.AddAsync(room, It.IsAny<CancellationToken>()));
        _roomRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mapper.Setup(m => m.Map<RoomResponse>(room))
            .Returns(roomResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(roomResponse);
        _roomRepository.Verify(r => r.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Once);
        _roomRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRoomCommandHandler_RoomTypeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreateRoomCommand>();

        _roomTypeRepository.Setup(r => r.IsExistsByIdAsync(command.RoomTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomTypeErrors.RoomTypeNotFound());
        _roomRepository.Verify(r => r.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateRoomCommandHandler_RoomAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreateRoomCommand>();
        var room = _fixture.Create<Room>();

        _roomTypeRepository.Setup(r => r.IsExistsByIdAsync(command.RoomTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Room>(command))
            .Returns(room);
        _roomRepository.Setup(r => r.IsExistAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomErrors.RoomAlreadyExists());
        _roomRepository.Verify(r => r.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
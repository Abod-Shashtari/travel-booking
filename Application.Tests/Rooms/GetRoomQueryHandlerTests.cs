using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Rooms.GetRoom;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Errors;

namespace Application.Tests.Rooms;

public class GetRoomQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Room>> _roomRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly GetRoomQueryHandler _handler;

    public GetRoomQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _roomRepository = new Mock<IRepository<Room>>();
        _mapper = new Mock<IMapper>();

        _handler = new GetRoomQueryHandler(_roomRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetRoomQueryHandler_RoomExists_ShouldReturnSuccess()
    {
        // Arrange
        var query = _fixture.Create<GetRoomQuery>();
        var room = _fixture.Create<Room>();
        var roomResponse = _fixture.Create<RoomResponse>();

        _roomRepository.Setup(r => r.GetByIdAsync(query.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        _mapper.Setup(m => m.Map<RoomResponse>(It.IsAny<Room>()))
            .Returns(roomResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(roomResponse);
        _roomRepository.Verify(r => r.GetByIdAsync(query.RoomId, It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(m => m.Map<RoomResponse>(It.IsAny<Room>()), Times.Once);
    }

    [Fact]
    public async Task GetRoomQueryHandler_RoomDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var query = _fixture.Create<GetRoomQuery>();

        _roomRepository.Setup(r => r.GetByIdAsync(query.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomErrors.RoomNotFound());
        _mapper.Verify(m => m.Map<RoomResponse>(It.IsAny<Room>()), Times.Never);
    }
}
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Rooms.GetRooms;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Rooms.Entities;

namespace Application.Tests.Rooms;

public class GetRoomsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Room>> _roomRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly GetRoomsQueryHandler _handler;

    public GetRoomsQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _roomRepository = new Mock<IRepository<Room>>();
        _mapper = new Mock<IMapper>();
        _handler = new GetRoomsQueryHandler(_mapper.Object, _roomRepository.Object);
    }

    [Fact]
    public async Task GetRoomsQueryHandler_ValidRequest_ShouldReturnSuccessWithPaginatedRooms()
    {
        // Arrange
        var query = _fixture.Create<GetRoomsQuery>();
        var rooms = _fixture.CreateMany<Room>(query.PageSize).ToList();
        var mappedRoomResponses = _fixture.CreateMany<RoomResponse>(query.PageSize).ToList();
        var paginatedRooms = new PaginatedList<Room>(rooms, 50, query.PageSize, query.PageNumber);
        
        _roomRepository.Setup(r => r.GetPaginatedListAsync(
                It.IsAny<RoomsByRoomTypeIdSpecification>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedRooms);

        _mapper.Setup(m => m.Map<List<RoomResponse>>(rooms))
            .Returns(mappedRoomResponses);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.Should().BeEquivalentTo(mappedRoomResponses);
        result.Value.TotalCount.Should().Be(paginatedRooms.TotalCount);
        result.Value.CurrentPage.Should().Be(paginatedRooms.CurrentPage);
        result.Value.PageSize.Should().Be(paginatedRooms.PageSize);
    }
}
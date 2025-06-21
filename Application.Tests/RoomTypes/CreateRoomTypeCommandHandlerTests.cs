using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.RoomTypes.CreateRoomType;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace Application.Tests.RoomTypes;

public class CreateRoomTypeCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IRepository<RoomType>> _roomTypeRepository;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly CreateRoomTypeCommandHandler _handler;

    public CreateRoomTypeCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        _mapper = new Mock<IMapper>();
        _roomTypeRepository = new Mock<IRepository<RoomType>>();
        _hotelRepository = new Mock<IRepository<Hotel>>();

        _handler = new CreateRoomTypeCommandHandler(
            _mapper.Object, 
            _roomTypeRepository.Object, 
            _hotelRepository.Object
        );
    }

    [Fact]
    public async Task CreateRoomTypeCommandHandler_HotelNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreateRoomTypeCommand>();
        _hotelRepository
            .Setup(r => r.IsExistsByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(HotelErrors.HotelNotFound());
        _roomTypeRepository.Verify(r => r.AddAsync(It.IsAny<RoomType>(), It.IsAny<CancellationToken>()), Times.Never);
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateRoomTypeCommandHandler_DuplicateRoomType_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreateRoomTypeCommand>();
        var roomType = _fixture.Create<RoomType>();

        _hotelRepository
            .Setup(r => r.IsExistsByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapper
            .Setup(m => m.Map<CreateRoomTypeCommand, RoomType>(command))
            .Returns(roomType);

        _roomTypeRepository
            .Setup(r => r.IsExistAsync(roomType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomTypeErrors.RoomTypeAlreadyExists());
        _roomTypeRepository.Verify(r => r.AddAsync(It.IsAny<RoomType>(), It.IsAny<CancellationToken>()), Times.Never);
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateRoomTypeCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<CreateRoomTypeCommand>();
        var roomType = _fixture.Create<RoomType>();
        var response = _fixture.Create<RoomTypeResponse>();

        _hotelRepository
            .Setup(r => r.IsExistsByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapper
            .Setup(m => m.Map<CreateRoomTypeCommand, RoomType>(command))
            .Returns(roomType);

        _roomTypeRepository
            .Setup(r => r.IsExistAsync(roomType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _roomTypeRepository
            .Setup(r => r.AddAsync(roomType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        _roomTypeRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapper
            .Setup(m => m.Map<RoomTypeResponse>(roomType))
            .Returns(response);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(response);
        _roomTypeRepository.Verify(r => r.AddAsync(roomType, It.IsAny<CancellationToken>()), Times.Once);
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Amenities.AddAmenityToRoomType;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Amenities.Errors;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace Application.Tests.Amenities;

public class AddAmenityToRoomTypeCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Amenity>> _amenityRepository;
    private readonly Mock<IRepository<RoomType>> _roomTypeRepository;
    private readonly AddAmenityToRoomTypeCommandHandler _handler;

    public AddAmenityToRoomTypeCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _amenityRepository = new Mock<IRepository<Amenity>>();
        _roomTypeRepository = new Mock<IRepository<RoomType>>();
        _handler = new AddAmenityToRoomTypeCommandHandler(_amenityRepository.Object, _roomTypeRepository.Object);
    }

    [Fact]
    public async Task AddAmenityToRoomTypeCommandHandler_AmenityNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<AddAmenityToRoomTypeCommand>();

        _amenityRepository.Setup(r => r.GetByIdAsync(command.AmenityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Amenity?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AmenityErrors.AmenityNotFound());
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddAmenityToRoomTypeCommandHandler_RoomTypeNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<AddAmenityToRoomTypeCommand>();
        var amenity = _fixture.Create<Amenity>();

        _amenityRepository.Setup(r => r.GetByIdAsync(command.AmenityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(amenity);

        _roomTypeRepository.Setup(r => r.GetByIdAsync(command.RoomTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoomType?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomTypeErrors.RoomTypeNotFound());
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddAmenityToRoomTypeCommandHandler_ValidRequest_ShouldAddAmenityAndReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<AddAmenityToRoomTypeCommand>();
        var amenity = _fixture.Create<Amenity>();
        var roomType = _fixture.Build<RoomType>()
            .Without(rt => rt.Amenities)
            .Do(rt => rt.Amenities = new List<Amenity>())
            .Create();

        _amenityRepository.Setup(r => r.GetByIdAsync(command.AmenityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(amenity);

        _roomTypeRepository.Setup(r => r.GetByIdAsync(command.RoomTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roomType);

        _roomTypeRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        roomType.Amenities.Should().Contain(amenity);
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
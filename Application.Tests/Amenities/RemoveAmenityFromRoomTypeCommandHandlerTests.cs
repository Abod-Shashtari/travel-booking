using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Amenities.RemoveAmenityFromRoomType;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Amenities.Errors;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace Application.Tests.Amenities;

public class RemoveAmenityFromRoomTypeCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Amenity>> _amenityRepository;
    private readonly Mock<IRepository<RoomType>> _roomTypeRepository;
    private readonly RemoveAmenityFromRoomTypeCommandHandler _handler;

    public RemoveAmenityFromRoomTypeCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _amenityRepository = new Mock<IRepository<Amenity>>();
        _roomTypeRepository = new Mock<IRepository<RoomType>>();

        _handler = new RemoveAmenityFromRoomTypeCommandHandler(
            _amenityRepository.Object,
            _roomTypeRepository.Object
        );
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldRemoveAmenityAndReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<RemoveAmenityFromRoomTypeCommand>();
        var amenity = _fixture.Create<Amenity>();
        var roomType = _fixture.Build<RoomType>()
            .With(rt => rt.Amenities, new List<Amenity> { amenity })
            .Create();

        _amenityRepository.Setup(r => r.GetByIdAsync(command.AmenityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(amenity);

        _roomTypeRepository.Setup(r => r.GetByIdAsync(
                command.RoomTypeId,
                It.IsAny<RoomTypesWithAmenitySpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(roomType);

        _roomTypeRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        roomType.Amenities.Should().NotContain(amenity);
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AmenityDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<RemoveAmenityFromRoomTypeCommand>();

        _amenityRepository.Setup(r => r.GetByIdAsync(command.AmenityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Amenity)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AmenityErrors.AmenityNotFound());

        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RoomTypeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<RemoveAmenityFromRoomTypeCommand>();
        var amenity = _fixture.Create<Amenity>();

        _amenityRepository.Setup(r => r.GetByIdAsync(command.AmenityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(amenity);

        _roomTypeRepository.Setup(r => r.GetByIdAsync(
                command.RoomTypeId,
                It.IsAny<RoomTypesWithAmenitySpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoomType)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomTypeErrors.RoomTypeNotFound());
        _roomTypeRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Amenities.DeleteAmenity;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Amenities.Errors;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Amenities;

public class DeleteAmenityCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Amenity>> _amenityRepository;
    private readonly DeleteAmenityCommandHandler _handler;

    public DeleteAmenityCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _amenityRepository = new Mock<IRepository<Amenity>>();
        _handler = new DeleteAmenityCommandHandler(_amenityRepository.Object);
    }

    [Fact]
    public async Task DeleteAmenityCommandHandler_AmenityNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<DeleteAmenityCommand>();

        _amenityRepository.Setup(r => r.GetByIdAsync(command.AmenityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Amenity?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AmenityErrors.AmenityNotFound());
        _amenityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAmenityCommandHandler_ValidRequest_ShouldDeleteAndReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<DeleteAmenityCommand>();
        var amenity = _fixture.Create<Amenity>();

        _amenityRepository.Setup(r => r.GetByIdAsync(command.AmenityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(amenity);

        _amenityRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _amenityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
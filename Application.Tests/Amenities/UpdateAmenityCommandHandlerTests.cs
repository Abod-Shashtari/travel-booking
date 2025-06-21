using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Amenities.UpdateAmenity;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Amenities.Errors;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Amenities;

public class UpdateAmenityCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Amenity>> _amenityRepositoryMock;
    private readonly UpdateAmenityCommandHandler _handler;

    public UpdateAmenityCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var mapper = new Mock<IMapper>();
        _amenityRepositoryMock = new Mock<IRepository<Amenity>>();

        _handler = new UpdateAmenityCommandHandler(mapper.Object, _amenityRepositoryMock.Object);
    }

    [Fact]
    public async Task UpdateAmenityCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<UpdateAmenityCommand>();
        var amenity = _fixture.Create<Amenity>();

        _amenityRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.AmenityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(amenity);

        _amenityRepositoryMock
            .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _amenityRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAmenityCommandHandler_AmenityNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<UpdateAmenityCommand>();

        _amenityRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.AmenityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Amenity)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AmenityErrors.AmenityNotFound());
        _amenityRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
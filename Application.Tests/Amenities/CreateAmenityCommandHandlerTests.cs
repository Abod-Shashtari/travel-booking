using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Amenities.CreateAmenity;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Amenities.Errors;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Amenities;

public class CreateAmenityCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Amenity>> _amenityRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly CreateAmenityCommandHandler _handler;

    public CreateAmenityCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _amenityRepository = new Mock<IRepository<Amenity>>();
        _mapper = new Mock<IMapper>();
        _handler = new CreateAmenityCommandHandler(_amenityRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task CreateAmenityCommandHandler_AmenityAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreateAmenityCommand>();
        var amenity = _fixture.Create<Amenity>();

        _mapper.Setup(m => m.Map<Amenity>(command)).Returns(amenity);
        _amenityRepository.Setup(r => r.IsExistAsync(amenity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AmenityErrors.AmenityAlreadyExists());
        _amenityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAmenityCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<CreateAmenityCommand>();
        var amenity = _fixture.Create<Amenity>();
        var amenityResponse = _fixture.Create<AmenityResponse>();

        _mapper.Setup(m => m.Map<Amenity>(command)).Returns(amenity);
        _amenityRepository.Setup(r => r.IsExistAsync(amenity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _amenityRepository.Setup(r => r.AddAsync(amenity, It.IsAny<CancellationToken>()));
        _amenityRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mapper.Setup(m => m.Map<AmenityResponse>(amenity)).Returns(amenityResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(amenityResponse);
        _amenityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
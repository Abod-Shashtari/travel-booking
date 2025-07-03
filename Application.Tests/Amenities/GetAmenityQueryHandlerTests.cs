using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Amenities.GetAmenity;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Amenities.Errors;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Amenities;

public class GetAmenityQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Amenity>> _amenityRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly GetAmenityQueryHandler _handler;

    public GetAmenityQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _amenityRepository = new Mock<IRepository<Amenity>>();
        _mapper = new Mock<IMapper>();
        _handler = new GetAmenityQueryHandler(_amenityRepository.Object, _mapper.Object);
    }
    
    [Fact]
    public async Task GetAmenityQueryHandler_AmenityNotFound_ShouldReturnFailure()
    {
        // Arrange
        var amenityId = Guid.NewGuid();
        var query  = new GetAmenityQuery(amenityId);

        _amenityRepository
            .Setup(r => r.GetByIdAsync(
                amenityId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Amenity?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AmenityErrors.AmenityNotFound());
    }

    [Fact]
    public async Task GetAmenityQueryHandler_AmenityExists_ShouldReturnSuccess()
    {
        // Arrange
        var amenityId = Guid.NewGuid();
        var query = new GetAmenityQuery(amenityId);
        var amenity = _fixture.Create<Amenity>();
        var amenityResponse = _fixture.Create<AmenityResponse>();

        _amenityRepository.Setup(r => r.GetByIdAsync(query.AmenityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(amenity);

        _mapper.Setup(m => m.Map<AmenityResponse>(It.IsAny<Amenity>()))
            .Returns(amenityResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(amenityResponse);
    }

}
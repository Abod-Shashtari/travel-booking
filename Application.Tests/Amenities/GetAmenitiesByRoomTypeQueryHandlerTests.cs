using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Amenities.GetAmenitiesByRoomType;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Amenities;

public class GetAmenitiesByRoomTypeQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Amenity>> _amenityRepository;
    private readonly GetAmenitiesByRoomTypeQueryHandler _handler;
    private readonly Mock<IMapper> _mapper;

    public GetAmenitiesByRoomTypeQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _amenityRepository = new Mock<IRepository<Amenity>>();
        _mapper = new Mock<IMapper>();
        _handler = new GetAmenitiesByRoomTypeQueryHandler(_amenityRepository.Object,_mapper.Object);
    }

    [Fact]
    public async Task GetAmenitiesByRoomTypeQueryHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var query = _fixture.Create<GetAmenitiesByRoomTypeQuery>();
        var amenities = _fixture.CreateMany<Amenity>(3).ToList();
        var paginatedAmenities = new PaginatedList<Amenity>(amenities, amenities.Count, query.PageNumber, query.PageSize);
        var mappedAmenityResponses = _fixture.CreateMany<AmenityResponse>(3).ToList();
        
        _amenityRepository
            .Setup(r => r.GetPaginatedListAsync(
                It.IsAny<AmenitiesByRoomTypeSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedAmenities);
        _mapper
            .Setup(m => m.Map<List<AmenityResponse>>(It.IsAny<List<Amenity>>()))
            .Returns(mappedAmenityResponses);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.Should().BeEquivalentTo(mappedAmenityResponses);
        result.Value.Data.Should().HaveCount(amenities.Count);
        result.Value.TotalCount.Should().Be(amenities.Count);
    }
}
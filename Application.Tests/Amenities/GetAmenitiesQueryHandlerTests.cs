using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Amenities.GetAmenities;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Amenities;

public class GetAmenitiesQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Amenity>> _amenityRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly GetAmenitiesQueryHandler _handler;

    public GetAmenitiesQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _amenityRepository = new Mock<IRepository<Amenity>>();
        _mapper = new Mock<IMapper>();

        _handler = new GetAmenitiesQueryHandler(_mapper.Object, _amenityRepository.Object);
    }

    [Fact]
    public async Task GetAmenitiesQueryHandler_ShouldReturnSuccessWithPaginatedList()
    {
        // Arrange
        var query = _fixture.Create<GetAmenitiesQuery>();
        var amenities = _fixture.CreateMany<Amenity>(5).ToList();
        var mappedAmenityResponses = _fixture.CreateMany<AmenityResponse>(5).ToList();
        var paginatedAmenities = new PaginatedList<Amenity>(amenities, amenities.Count, query.PageSize, query.PageNumber);

        _amenityRepository.Setup(r => r.GetPaginatedListAsync(
                It.IsAny<PaginationSpecification<Amenity>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedAmenities);

        _mapper.Setup(m => m.Map<List<AmenityResponse>>(amenities))
            .Returns(mappedAmenityResponses);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.Should().BeEquivalentTo(mappedAmenityResponses);
        result.Value.TotalCount.Should().Be(paginatedAmenities.TotalCount);
        result.Value.CurrentPage.Should().Be(paginatedAmenities.CurrentPage);
        result.Value.PageSize.Should().Be(paginatedAmenities.PageSize);
    }
}
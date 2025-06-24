using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.GetTrendingCities;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Cities;

public class GetTrendingCitiesQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<City>> _cityRepository;
    private readonly GetTrendingCitiesQueryHandler _handler;

    public GetTrendingCitiesQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _cityRepository = new Mock<IRepository<City>>();
        _handler = new GetTrendingCitiesQueryHandler(_cityRepository.Object);
    }

    [Fact]
    public async Task GetTrendingCitiesQueryHandler_ValidRequest_ShouldReturnPaginatedCityResponse()
    {
        // Arrange
        var query = _fixture.Create<GetTrendingCitiesQuery>();

        var cityResponses = _fixture.CreateMany<CityResponse>(5).ToList();
        var paginatedResponse = new PaginatedList<CityResponse>(
            cityResponses,
            5,
            5,
            1
        );

        _cityRepository.Setup(r =>
                r.GetPaginatedListAsync(
                    It.IsAny<GetTrendingCitiesSpecification>(),
                    It.IsAny<Expression<Func<City, CityResponse>>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.Should().BeEquivalentTo(paginatedResponse.Data);
        result.Value.TotalCount.Should().Be(paginatedResponse.TotalCount);
        result.Value.PageSize.Should().Be(paginatedResponse.PageSize);
        result.Value.CurrentPage.Should().Be(paginatedResponse.CurrentPage);
    }
}
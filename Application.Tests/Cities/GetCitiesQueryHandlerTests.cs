using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.GetCities;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;

namespace Application.Tests.Cities;

public class GetCitiesQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<City>> _cityRepository;
    private readonly GetCitiesQueryHandler _handler;

    public GetCitiesQueryHandlerTests()
    {
        _fixture   = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _cityRepository  = new Mock<IRepository<City>>();
        _handler   = new GetCitiesQueryHandler(_cityRepository.Object);
    }

    [Fact]
    public async Task GetCitiesQueryHandler_ValidRequest_ShouldReturnPaginatedResponses()
    {
        // Arrange
        var query = _fixture.Create<GetCitiesQuery>();

        List<City> cities =
        [
            _fixture.Build<City>()
                .With(c => c.Hotels, new List<Hotel> { new Hotel(), new Hotel() })
                .Create(),
            _fixture.Build<City>()
                .With(c => c.Hotels, new List<Hotel> { new Hotel()})
                .Create(),
            _fixture.Build<City>()
                .With(c => c.Hotels, new List<Hotel>())
                .Create(),
        ];
        
        Expression<Func<City, CityResponse>> selector = city => new CityResponse(
            city.Id,
            city.Name,
            city.Country,
            city.PostOffice,
            city.Hotels.Count,
            new ImageResponse(
                city.ThumbnailImageId,
                city.ThumbnailImage != null?city.ThumbnailImage.Url:""
            )
        );
        
        var cityResponses = cities.Select(selector.Compile()).ToList();
        var projectedList = new PaginatedList<CityResponse>(cityResponses,cityResponses.Count, query.PageNumber, query.PageSize);

        _cityRepository.Setup(r => r.GetPaginatedListAsync(
                It.IsAny<PaginationSpecification<City>>(),
                It.IsAny<Expression<Func<City, CityResponse>>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(projectedList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.Should().BeEquivalentTo(cityResponses);
        result.Value.TotalCount.Should().Be(projectedList.TotalCount);
        result.Value.CurrentPage.Should().Be(projectedList.CurrentPage);
        result.Value.PageSize.Should().Be(projectedList.PageSize);
    }
}
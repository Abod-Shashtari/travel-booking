using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.GetCity;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Cities.Errors;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Cities;

public class GetCityQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<City>> _cityRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly GetCityQueryHandler _handler;

    public GetCityQueryHandlerTests()
    {
        _fixture   = new Fixture();
        _cityRepository  = new Mock<IRepository<City>>();
        _mapper= new Mock<IMapper>();
        _handler   = new GetCityQueryHandler(_cityRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetCityQueryHandler_CityNotFound_ShouldReturnFailure()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var query  = new GetCityQuery(cityId);

        _cityRepository
            .Setup(r => r.GetByIdAsync(
                cityId,
                It.IsAny<Expression<Func<City, CityResponse>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CityResponse)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CityErrors.CityNotFound());
    }

    [Fact]
    public async Task GetCityQueryHandler_CityExists_ShouldReturnSuccess()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var query  = new GetCityQuery(cityId);

        var projected = _fixture.Build<CityResponse>()
            .With(x => x.Id, cityId)
            .Create();

        var mapped = _fixture.Build<CityResponse>()
            .With(x => x.Id, cityId)
            .Create();

        _cityRepository.Setup(r => r.GetByIdAsync(
                cityId,
                It.IsAny<Expression<Func<City, CityResponse>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projected);

        _mapper
            .Setup(m => m.Map<CityResponse>(projected))
            .Returns(mapped);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(mapped);
        _cityRepository.Verify(r => r.GetByIdAsync(
            cityId,
            It.IsAny<Expression<Func<City, CityResponse>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
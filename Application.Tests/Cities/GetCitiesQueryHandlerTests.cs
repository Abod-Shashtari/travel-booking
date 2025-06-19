using System.Linq.Expressions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.GetCities;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Cities;

public class GetCitiesQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<City>> _cityRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly GetCitiesQueryHandler _handler;

    public GetCitiesQueryHandlerTests()
    {
        _fixture   = new Fixture();
        _fixture.Customize<City>(c => c
            .Without(city => city.Hotels)
        );
        _cityRepository  = new Mock<IRepository<City>>();
        _mapper= new Mock<IMapper>();
        _handler   = new GetCitiesQueryHandler(_mapper.Object, _cityRepository.Object);
    }

    [Fact]
    public async Task GetCitiesCommandHandler_ValidRequest_ShouldReturnPaginatedResponses()
    {
        // Arrange
        var command = new GetCitiesQuery( 2, 3);
        const int page = 2;
        const int size = 3;
        const int total = 10;
        
        var data = new List<CityResponse>
        {
            _fixture.Create<CityResponse>(),
            _fixture.Create<CityResponse>(),
            _fixture.Create<CityResponse>()
        };
        
        var projectedList = new PaginatedList<CityResponse>(data, total, size, page);

        _cityRepository.Setup(r => r.GetPaginatedListAsync(
                It.Is<PaginationSpecification<City>>(
                    s => s.Skip== (page-1)*size && s.Take == size),
                It.IsAny<Expression<Func<City, CityResponse>>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(projectedList);

        _mapper.Setup(m => m.Map<List<CityResponse>>(data))
            .Returns(data);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        result.Value.Should().NotBeNull();
        result.Value!.TotalCount.Should().Be(total);
        result.Value!.CurrentPage.Should().Be(page);
        result.Value!.PageSize.Should().Be(size);
        result.Value!.Data.Should().BeEquivalentTo(data);
    }
}
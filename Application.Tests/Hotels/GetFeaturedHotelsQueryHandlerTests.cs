using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Hotels.GetFeaturedHotels;
using TravelBooking.Application.Hotels.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;

namespace Application.Tests.Hotels;

public class GetFeaturedHotelsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly GetFeaturedHotelsQueryHandler _handler;

    public GetFeaturedHotelsQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _hotelRepository = new Mock<IRepository<Hotel>>();
        _handler = new GetFeaturedHotelsQueryHandler(_hotelRepository.Object);
    }

    [Fact]
    public async Task GetFeaturedHotelsQueryHandler_ValidRequest_ShouldReturnPaginatedHotelResponse()
    {
        // Arrange
        var query = _fixture.Create<GetFeaturedHotelsQuery>();

        var hotelResponses = _fixture.CreateMany<HotelResponse>(5).ToList();
        var paginatedHotelResponses = new PaginatedList<HotelResponse>(
            hotelResponses,
            hotelResponses.Count,
            hotelResponses.Count,
            1
        );

        _hotelRepository.Setup(r => r.GetPaginatedListAsync(
                It.IsAny<GetFeaturedHotelsSpecification>(),
                It.IsAny<Expression<Func<Hotel, HotelResponse>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedHotelResponses);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.Should().BeEquivalentTo(paginatedHotelResponses.Data);
        result.Value.TotalCount.Should().Be(paginatedHotelResponses.TotalCount);
        result.Value.PageSize.Should().Be(paginatedHotelResponses.PageSize);
        result.Value.CurrentPage.Should().Be(paginatedHotelResponses.CurrentPage);
    }
}
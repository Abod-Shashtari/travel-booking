using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.UserActivity.GetRecentlyVisitedHotels;
using TravelBooking.Application.UserActivity.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.UserActivity.Entites;

namespace Application.Tests.UserActivity;

public class GetRecentlyVisitedHotelsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<HotelVisit>> _hotelVisitRepository;
    private readonly GetRecentlyVisitedHotelsQueryHandler _handler;

    public GetRecentlyVisitedHotelsQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _hotelVisitRepository = new Mock<IRepository<HotelVisit>>();
        _handler = new GetRecentlyVisitedHotelsQueryHandler(_hotelVisitRepository.Object);
    }

    [Fact]
    public async Task GetRecentlyVisitedHotelsQueryHandler_ValidRequest_ShouldReturnPaginatedHotelResponse()
    {
        // Arrange
        var query = _fixture.Create<GetRecentlyVisitedHotelsQuery>();

        var hotelResponses = _fixture.CreateMany<HotelResponse>(5).ToList();
        var paginatedResponse = new PaginatedList<HotelResponse>(
            hotelResponses,
            5,
            5,
            1
        );

        _hotelVisitRepository
            .Setup(r => r.GetPaginatedListAsync(
                It.IsAny<GetRecentlyVisitedHotelsSpecification>(),
                It.IsAny<Expression<Func<HotelVisit, HotelResponse>>>(),
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
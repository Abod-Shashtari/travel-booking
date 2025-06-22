using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Hotels.GetHotels;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;

namespace Application.Tests.Hotels;

public class GetHotelsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly GetHotelsQueryHandler _handler;

    public GetHotelsQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _hotelRepository = new Mock<IRepository<Hotel>>();
        _handler = new GetHotelsQueryHandler(_hotelRepository.Object);
    }

    [Fact]
    public async Task GetHotelsQueryHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var query = _fixture.Create<GetHotelsQuery>();
        var hotels=_fixture.CreateMany<Hotel>().ToList();

        Expression<Func<Hotel, HotelResponse>> selector = hotel => new HotelResponse(
            hotel.Id,
            hotel.Name,
            hotel.Location,
            hotel.Description,
            hotel.City!.Name,
            hotel.CityId,
            hotel.OwnerId,
            hotel.CreatedAt,
            hotel.ModifiedAt,
            new ImageResponse(
                hotel.ThumbnailImageId,
                hotel.ThumbnailImage != null?hotel.ThumbnailImage.Url:""
            )
        );
        
        var hotelResponses = hotels.Select(selector.Compile()).ToList();
        var projectedList = new PaginatedList<HotelResponse>(hotelResponses,hotelResponses.Count, query.PageNumber, query.PageSize);
        
        _hotelRepository
            .Setup(r => r.GetPaginatedListAsync(
                It.IsAny<HotelSearchSpecification>(),
                It.IsAny<Expression<Func<Hotel, HotelResponse>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectedList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.Should().BeEquivalentTo(hotelResponses);
        result.Value.TotalCount.Should().Be(projectedList.TotalCount);
        result.Value.CurrentPage.Should().Be(projectedList.CurrentPage);
        result.Value.PageSize.Should().Be(projectedList.PageSize);
    }
}
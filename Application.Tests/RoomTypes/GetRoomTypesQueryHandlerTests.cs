using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.RoomTypes.GetRoomTypes;
using TravelBooking.Application.RoomTypes.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.RoomTypes.Entities;

namespace Application.Tests.RoomTypes;

public class GetRoomTypesQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<RoomType>> _roomTypeRepository;
    private readonly GetRoomTypesQueryHandler _handler;

    public GetRoomTypesQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _roomTypeRepository = new Mock<IRepository<RoomType>>();
        _handler = new GetRoomTypesQueryHandler(_roomTypeRepository.Object);
    }

    [Fact]
    public async Task GetRoomTypesQueryHandler_ShouldReturnSuccessWithPaginatedList()
    {
        // Arrange
        var query = _fixture.Create<GetRoomTypesQuery>();
        var roomTypes = _fixture.CreateMany<RoomType>(5).ToList();
        var paginatedRoomTypes = new PaginatedList<RoomType>(roomTypes, roomTypes.Count, query.PageSize, query.PageNumber);

        _roomTypeRepository.Setup(r => r.GetPaginatedListAsync(
                It.IsAny<IncludeDiscountsWithRoomTypeSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedRoomTypes);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.Should().HaveCount(roomTypes.Count);
        result.Value.TotalCount.Should().Be(paginatedRoomTypes.TotalCount);
        result.Value.CurrentPage.Should().Be(paginatedRoomTypes.CurrentPage);
        result.Value.PageSize.Should().Be(paginatedRoomTypes.PageSize);
    }

    [Fact]
    public async Task GetRoomTypesQueryHandler_ShouldCalculateDiscountedPrice_WhenDiscountsExist()
    {
        // Arrange
        var query = _fixture.Create<GetRoomTypesQuery>();
        var currentDate = DateTime.Now;
        const decimal originalPrice = 100m;
        
        var roomType = _fixture.Build<RoomType>()
            .With(rt => rt.PricePerNight, originalPrice)
            .With(rt => rt.Discounts, new List<Discount>
            {
                _fixture.Build<Discount>()
                    .With(d => d.StartDate, currentDate.AddDays(-1))
                    .With(d => d.EndDate, currentDate.AddDays(1))
                    .With(d => d.Percentage, 20m)
                    .Create(),
                _fixture.Build<Discount>()
                .With(d => d.StartDate, currentDate.AddDays(-2))
                .With(d => d.EndDate, currentDate.AddDays(-1))
                .With(d => d.Percentage, 20m)
                .Create()
            })
            .Create();

        var roomTypes = new List<RoomType> { roomType };
        var paginatedRoomTypes = new PaginatedList<RoomType>(roomTypes, roomTypes.Count, query.PageSize, query.PageNumber);

        _roomTypeRepository.Setup(r => r.GetPaginatedListAsync(
                It.IsAny<IncludeDiscountsWithRoomTypeSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedRoomTypes);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.First().PricePerNight.Should().Be(originalPrice);
        result.Value.Data.First().DiscountedPricePerNight.Should().Be(80m);
    }
}
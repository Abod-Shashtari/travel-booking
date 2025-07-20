using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.RoomTypes.GetRoomTypesOfHotel;
using TravelBooking.Application.RoomTypes.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.RoomTypes.Entities;

namespace Application.Tests.RoomTypes;

public class GetRoomTypesOfHotelQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<RoomType>> _roomTypeRepository;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly GetRoomTypesOfHotelQueryHandler _handler;

    public GetRoomTypesOfHotelQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _roomTypeRepository = new Mock<IRepository<RoomType>>();
        _hotelRepository = new Mock<IRepository<Hotel>>();
        _handler = new GetRoomTypesOfHotelQueryHandler(_roomTypeRepository.Object, _hotelRepository.Object);
    }

    [Fact]
    public async Task GetRoomTypesOfHotelQueryHandler_HotelNotFound_ShouldReturnFailure()
    {
        // Arrange
        var query = _fixture.Create<GetRoomTypesOfHotelQuery>();

        _hotelRepository.Setup(h => h.IsExistsByIdAsync(
            query.HotelId,
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(false);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(HotelErrors.HotelNotFound());
    }
    [Fact]
    public async Task GetRoomTypesOfHotelQueryHandler_NoDiscounts_ShouldReturnSuccessWithPaginatedList()
    {
        // Arrange
        var query = _fixture.Create<GetRoomTypesOfHotelQuery>();
        var roomTypes = _fixture.CreateMany<RoomType>(5).ToList();
        var paginatedRoomTypes = new PaginatedList<RoomType>(roomTypes, roomTypes.Count, query.PageSize, query.PageNumber);

        _hotelRepository.Setup(h => h.IsExistsByIdAsync(
            query.HotelId,
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(true);
        
        _roomTypeRepository.Setup(r => r.GetPaginatedListAsync(
                It.IsAny<HotelRoomTypesIncludingDiscountsSpecification>(),
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
    public async Task GetRoomTypesOfHotelQueryHandler_ShouldCalculateDiscountedPrice_WhenDiscountsExist()
    {
        // Arrange
        var query = _fixture.Create<GetRoomTypesOfHotelQuery>();
        var currentDate = DateTime.Now;
        const decimal originalPrice = 100m;
        
        _hotelRepository.Setup(h => h.IsExistsByIdAsync(
            query.HotelId,
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(true);
        
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
                It.IsAny<HotelRoomTypesIncludingDiscountsSpecification>(),
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
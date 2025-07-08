using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.RoomTypes.GetRoomType;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.RoomTypes.Errors;

namespace Application.Tests.RoomTypes;

public class GetRoomTypeQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<RoomType>> _roomTypeRepository;
    private readonly GetRoomTypeQueryHandler _handler;

    public GetRoomTypeQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _roomTypeRepository = new Mock<IRepository<RoomType>>();
        _handler = new GetRoomTypeQueryHandler(_roomTypeRepository.Object);
    }

    [Fact]
    public async Task GetRoomTypeQueryHandler_RoomTypeExists_ShouldReturnSuccessWithDiscountedPrice()
    {
        // Arrange
        var query = _fixture.Create<GetRoomTypeQuery>();
        var roomType = _fixture.Build<RoomType>()
            .With(r => r.Discounts, new List<Discount>
            {
                new Discount { StartDate = DateTime.Now.AddDays(-2), EndDate = DateTime.Now.AddDays(2), Percentage = 10 },
                new Discount { StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1), Percentage = 20 }
            })
            .Create();

        var expectedDiscountedPrice = roomType.PricePerNight * (decimal)(1 - 0.10) * (decimal)(1 - 0.20);

        _roomTypeRepository
            .Setup(r => r.GetByIdAsync(query.RoomTypeId, It.IsAny<ISpecification<RoomType>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(roomType);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(roomType.Id);
        result.Value.DiscountedPricePerNight.Should().Be(expectedDiscountedPrice);
    }

    [Fact]
    public async Task GetRoomTypeQueryHandler_RoomTypeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var query = _fixture.Create<GetRoomTypeQuery>();

        _roomTypeRepository
            .Setup(r => r.GetByIdAsync(query.RoomTypeId, It.IsAny<ISpecification<RoomType>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RoomType?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoomTypeErrors.RoomTypeNotFound());
    }

    [Fact]
    public async Task GetRoomTypeQueryHandler_NoValidDiscounts_ShouldReturnOriginalPriceAsDiscountedPrice()
    {
        // Arrange
        var query = _fixture.Create<GetRoomTypeQuery>();
        var roomType = _fixture.Build<RoomType>()
            .With(r => r.Discounts, new List<Discount>
            {
                new Discount { StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(-5), Percentage = 30 },
                new Discount { StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(10), Percentage = 40 }
            })
            .Create();

        _roomTypeRepository
            .Setup(r => r.GetByIdAsync(query.RoomTypeId, It.IsAny<ISpecification<RoomType>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(roomType);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.DiscountedPricePerNight.Should().Be(roomType.PricePerNight);
    }
}
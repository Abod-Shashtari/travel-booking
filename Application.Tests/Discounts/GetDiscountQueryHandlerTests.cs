using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Discounts.GetDiscount;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;

namespace Application.Tests.Discounts;

public class GetDiscountQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Discount>> _discountRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly GetDiscountQueryHandler _handler;

    public GetDiscountQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _discountRepository = new Mock<IRepository<Discount>>();
        _mapper = new Mock<IMapper>();
        _handler = new GetDiscountQueryHandler(_discountRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetDiscountQueryHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<GetDiscountQuery>();
        var discountEntity = _fixture.Create<Discount>();
        var discountResponse = _fixture.Create<DiscountResponse>();

        _discountRepository
            .Setup(r => r.GetByIdAsync(command.DiscountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(discountEntity);

        _mapper
            .Setup(m => m.Map<DiscountResponse>(discountEntity))
            .Returns(discountResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(discountResponse);
    }

    [Fact]
    public async Task GetDiscountQueryHandler_DiscountNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<GetDiscountQuery>();

        _discountRepository
            .Setup(r => r.GetByIdAsync(command.DiscountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Discount)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
    }
}
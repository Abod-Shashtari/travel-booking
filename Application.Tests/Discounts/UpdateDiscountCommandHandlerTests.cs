using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Discounts.UpdateDiscount;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;

namespace Application.Tests.Discounts;

public class UpdateDiscountCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Discount>> _discountRepository;
    private readonly UpdateDiscountCommandHandler _handler;

    public UpdateDiscountCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _discountRepository = new Mock<IRepository<Discount>>();
        var mapper = new Mock<IMapper>();
        _handler = new UpdateDiscountCommandHandler(mapper.Object, _discountRepository.Object);
    }

    [Fact]
    public async Task UpdateDiscountCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<UpdateDiscountCommand>();
        var discountEntity = _fixture.Create<Discount>();

        _discountRepository
            .Setup(r => r.GetByIdAsync(command.DiscountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(discountEntity);

        _discountRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _discountRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateDiscountCommandHandler_DiscountNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<UpdateDiscountCommand>();

        _discountRepository
            .Setup(r => r.GetByIdAsync(command.DiscountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Discount?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _discountRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
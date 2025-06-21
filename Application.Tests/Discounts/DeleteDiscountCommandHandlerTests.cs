using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Discounts.DeleteDiscount;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;

namespace Application.Tests.Discounts;

public class DeleteDiscountCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Discount>> _discountRepository;
    private readonly DeleteDiscountCommandHandler _handler;

    public DeleteDiscountCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _discountRepository = new Mock<IRepository<Discount>>();
        _handler = new DeleteDiscountCommandHandler(_discountRepository.Object);
    }

    [Fact]
    public async Task DeleteDiscountCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<DeleteDiscountCommand>();
        var discountEntity = _fixture.Create<Discount>();

        _discountRepository
            .Setup(r => r.GetByIdAsync(command.DiscountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(discountEntity);
        _discountRepository
            .Setup(r => r.Delete(discountEntity));
        _discountRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _discountRepository.Verify(r => r.Delete(discountEntity), Times.Once);
        _discountRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteDiscountCommandHandler_DiscountNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<DeleteDiscountCommand>();

        _discountRepository
            .Setup(r => r.GetByIdAsync(command.DiscountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Discount)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        _discountRepository.Verify(r => r.Delete(It.IsAny<Discount>()), Times.Never);
        _discountRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
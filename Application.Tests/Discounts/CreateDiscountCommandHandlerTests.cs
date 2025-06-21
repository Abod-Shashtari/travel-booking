using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Discounts.CreateDiscount;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;

namespace Application.Tests.Discounts;

public class CreateDiscountCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Discount>> _discountRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly CreateDiscountCommandHandler _handler;

    public CreateDiscountCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _discountRepository = new Mock<IRepository<Discount>>();
        _mapper = new Mock<IMapper>();
        _handler = new CreateDiscountCommandHandler(_discountRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task CreateDiscountCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<CreateDiscountCommand>();
        var discountEntity = _fixture.Create<Discount>();
        var discountResponse = _fixture.Create<DiscountResponse>();
        var expectedId = _fixture.Create<Guid>();

        _mapper
            .Setup(m => m.Map<Discount>(command))
            .Returns(discountEntity);
        _discountRepository
            .Setup(r => r.IsExistAsync(discountEntity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _discountRepository
            .Setup(r => r.AddAsync(discountEntity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);
        _discountRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mapper
            .Setup(m => m.Map<DiscountResponse>(discountEntity))
            .Returns(discountResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(discountResponse);
        _discountRepository.Verify(r => r.AddAsync(discountEntity, It.IsAny<CancellationToken>()), Times.Once);
        _discountRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateDiscountCommandHandler_DiscountAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreateDiscountCommand>();
        var discountEntity = _fixture.Create<Discount>();

        _mapper
            .Setup(m => m.Map<Discount>(command))
            .Returns(discountEntity);
        _discountRepository
            .Setup(r => r.IsExistAsync(discountEntity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        _discountRepository.Verify(r => r.AddAsync(It.IsAny<Discount>(), It.IsAny<CancellationToken>()), Times.Never);
        _discountRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
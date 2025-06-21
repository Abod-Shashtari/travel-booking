using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Discounts.GetDiscounts;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Discounts.Entities;

namespace Application.Tests.Discounts;

public class GetDiscountsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Discount>> _discountRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly GetDiscountsQueryHandler _handler;

    public GetDiscountsQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _discountRepository = new Mock<IRepository<Discount>>();
        _mapper = new Mock<IMapper>();
        _handler = new GetDiscountsQueryHandler(_mapper.Object, _discountRepository.Object);
    }

    [Fact]
    public async Task GetDiscountsQueryHandler_ValidRequest_ShouldReturnSuccessWithPaginatedList()
    {
        // Arrange
        var query = _fixture.Create<GetDiscountsQuery>();
        var discounts = _fixture.CreateMany<Discount>(3).ToList();
        var mappedDiscountResponses = _fixture.CreateMany<DiscountResponse>(3).ToList();
        var paginatedDiscounts = new PaginatedList<Discount>(discounts, discounts.Count, query.PageSize, query.PageNumber);

        _discountRepository.Setup(r => r.GetPaginatedListAsync(
                It.IsAny<PaginationSpecification<Discount>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedDiscounts);

        _mapper.Setup(m => m.Map<List<DiscountResponse>>(discounts))
            .Returns(mappedDiscountResponses);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.Should().BeEquivalentTo(mappedDiscountResponses);
        result.Value.TotalCount.Should().Be(paginatedDiscounts.TotalCount);
        result.Value.CurrentPage.Should().Be(paginatedDiscounts.CurrentPage);
        result.Value.PageSize.Should().Be(paginatedDiscounts.PageSize);
    }
}
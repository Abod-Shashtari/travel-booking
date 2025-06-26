using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Application.Reviews.GetReviews;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Reviews.Entities;

namespace Application.Tests.Reviews;

public class GetReviewsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Review>> _reviewRepository;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly GetReviewsQueryHandler _handler;

    public GetReviewsQueryHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _reviewRepository = new Mock<IRepository<Review>>();
        _hotelRepository = new Mock<IRepository<Hotel>>();
        _mapper = new Mock<IMapper>();

        _handler = new GetReviewsQueryHandler(
            _reviewRepository.Object,
            _hotelRepository.Object,
            _mapper.Object
        );
    }

    [Fact]
    public async Task GetReviewsQueryHandler_HotelNotFound_ShouldReturnFailure()
    {
        // Arrange
        var query = _fixture.Create<GetReviewsQuery>();

        _hotelRepository
            .Setup(r => r.IsExistsByIdAsync(query.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(HotelErrors.HotelNotFound());
    }

    [Fact]
    public async Task GetReviewsQueryHandler_HotelExists_ShouldReturnPaginatedReviewResponse()
    {
        // Arrange
        var query = _fixture.Create<GetReviewsQuery>();

        var reviewEntities = _fixture.CreateMany<Review>(3).ToList();
        var reviewResponses = _fixture.CreateMany<ReviewResponse>(3).ToList();

        var paginatedReviews = new PaginatedList<Review>(reviewEntities, 3, query.PageSize, query.PageNumber);

        _hotelRepository
            .Setup(r => r.IsExistsByIdAsync(query.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _reviewRepository
            .Setup(r => r.GetPaginatedListAsync(It.IsAny<PaginationSpecification<Review>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedReviews);

        _mapper
            .Setup(m => m.Map<List<ReviewResponse>>(reviewEntities))
            .Returns(reviewResponses);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Data.Should().BeEquivalentTo(reviewResponses);
        result.Value.TotalCount.Should().Be(paginatedReviews.TotalCount);
        result.Value.PageSize.Should().Be(paginatedReviews.PageSize);
        result.Value.CurrentPage.Should().Be(paginatedReviews.CurrentPage);
    }
}
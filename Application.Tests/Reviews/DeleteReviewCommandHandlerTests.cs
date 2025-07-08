using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Application.Reviews.DeleteReview;
using TravelBooking.Application.Reviews.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Reviews.Errors;

namespace Application.Tests.Reviews;

public class DeleteReviewCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Review>> _reviewRepository;
    private readonly DeleteReviewCommandHandler _handler;

    public DeleteReviewCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _reviewRepository = new Mock<IRepository<Review>>();
        _handler = new DeleteReviewCommandHandler(_reviewRepository.Object);
    }

    [Fact]
    public async Task DeleteReviewCommandHandler_ReviewNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<DeleteReviewCommand>();

        _reviewRepository
            .Setup(r => r.GetByIdAsync(command.ReviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.ReviewNotFound());
        _reviewRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteReviewCommandHandler_UserNotOwner_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<DeleteReviewCommand>();
        var review = _fixture.Build<Review>()
            .With(r => r.Id, command.ReviewId)
            .With(r => r.UserId, Guid.NewGuid())
            .Create();

        _reviewRepository
            .Setup(r => r.GetByIdAsync(
                command.ReviewId,
                It.IsAny<IncludeHotelWithReviewSpecification>(),
                It.IsAny<CancellationToken>())
            ) .ReturnsAsync(review);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.NotAllowedToUpdateThisReview());
        _reviewRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(4.0, 4.0, null, 1)]
    [InlineData(3.5, 4.0, 3.0, 2)]
    [InlineData(4.5, 5.0, 4.0, 2)] 
    [InlineData(3.0, 2.0, 4.0, 2)]  
    public async Task DeleteReviewCommandHandler_ValidRequest_ShouldReturnSuccessAndUpdateHotelStarRating(
        double baseStarRating, 
        double deletedReviewRating, 
        double? resultStarRating,
        int totalReviewCount)
    {
        // Arrange
        var command = _fixture.Create<DeleteReviewCommand>();
        
        var hotel = _fixture.Build<Hotel>()
            .With(h => h.StarRating, baseStarRating)
            .Create();
        
        var reviewToDelete = _fixture.Build<Review>()
            .With(r => r.Id, command.ReviewId)
            .With(r => r.UserId, command.UserId)
            .With(r => r.HotelId, hotel.Id)
            .With(r => r.StarRating, deletedReviewRating)
            .With(r=>r.Hotel, hotel)
            .Create();

        _reviewRepository
            .Setup(r => r.GetByIdAsync(
                command.ReviewId,
                It.IsAny<IncludeHotelWithReviewSpecification>(),
                It.IsAny<CancellationToken>())
            ) .ReturnsAsync(reviewToDelete);

        var reviews = new PaginatedList<Review>([], totalReviewCount, 0, 1);
        
        _reviewRepository.Setup(r=>r.GetPaginatedListAsync(
            It.IsAny<PaginationSpecification<Review>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(reviews);

        _reviewRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        if (resultStarRating.HasValue)
            hotel.StarRating.Should().BeApproximately(resultStarRating.Value, 0.001);
        else
            hotel.StarRating.Should().BeNull();
        
        _reviewRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _reviewRepository.Verify(r => r.Delete(reviewToDelete), Times.Once);
    }
}
using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Reviews.DeleteReview;
using TravelBooking.Domain.Common.Interfaces;
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
            .With(r => r.UserId, Guid.NewGuid()) // different user
            .Create();

        _reviewRepository
            .Setup(r => r.GetByIdAsync(command.ReviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.NotAllowedToUpdateThisReview());
        _reviewRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteReviewCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<DeleteReviewCommand>();
        var review = _fixture.Build<Review>()
            .With(r => r.Id, command.ReviewId)
            .With(r => r.UserId, command.UserId)
            .Create();

        _reviewRepository
            .Setup(r => r.GetByIdAsync(command.ReviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);

        _reviewRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _reviewRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
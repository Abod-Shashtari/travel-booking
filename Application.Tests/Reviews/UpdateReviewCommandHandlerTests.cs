using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Reviews.Specifications;
using TravelBooking.Application.Reviews.UpdateReview;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Reviews.Errors;

namespace Application.Tests.Reviews;

public class UpdateReviewCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Review>> _reviewRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly UpdateReviewCommandHandler _handler;

    public UpdateReviewCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _reviewRepository = new Mock<IRepository<Review>>();
        _mapper = new Mock<IMapper>();

        _handler = new UpdateReviewCommandHandler(_reviewRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task UpdateReviewCommandHandler_ReviewNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<UpdateReviewCommand>();

        _reviewRepository
            .Setup(r => r.GetByIdAsync(command.ReviewId,
                It.IsAny<IncludeHotelWithReviewSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.ReviewNotFound());
        _reviewRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateReviewCommandHandler_NotOwner_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<UpdateReviewCommand>();
        var review = _fixture.Create<Review>();
        review.UserId = Guid.NewGuid();

        _reviewRepository
            .Setup(r => r.GetByIdAsync(command.ReviewId,
                It.IsAny<IncludeHotelWithReviewSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.NotAllowedToUpdateThisReview());
        _reviewRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateReviewCommandHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<UpdateReviewCommand>();
        var review = _fixture.Create<Review>();
        review.UserId = command.UserId;

        _reviewRepository
            .Setup(r => r.GetByIdAsync(command.ReviewId,
                It.IsAny<IncludeHotelWithReviewSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);

        _mapper
            .Setup(m => m.Map(command, review));

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
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Reviews.PostReview;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Reviews.Errors;

namespace Application.Tests.Reviews;

public class PostReviewCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Review>> _reviewRepository;
    private readonly Mock<IRepository<Hotel>> _hotelRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly PostReviewCommandHandler _handler;

    public PostReviewCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _reviewRepository = new Mock<IRepository<Review>>();
        _hotelRepository = new Mock<IRepository<Hotel>>();
        _mapper = new Mock<IMapper>();

        _handler = new PostReviewCommandHandler(
            _reviewRepository.Object,
            _mapper.Object,
            _hotelRepository.Object
        );
    }

    [Fact]
    public async Task PostReviewCommandHandler_HotelDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<PostReviewCommand>();

        _hotelRepository.Setup(r => r.IsExistsByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(HotelErrors.HotelNotFound());
        _reviewRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task PostReviewCommandHandler_ReviewAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<PostReviewCommand>();
        var review = _fixture.Create<Review>();

        _hotelRepository.Setup(r => r.IsExistsByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapper.Setup(m => m.Map<Review>(command)).Returns(review);

        var hotel = _fixture.Create<Hotel>();

        _hotelRepository.Setup(h => h.GetByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);
        
        _reviewRepository.Setup(r => r.IsExistAsync(review, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.ReviewAlreadyExists());
        _reviewRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(null, 4.0, 4.0)]
    [InlineData(3.0, 4.0, 3.5)]
    public async Task PostReviewCommandHandler_ValidRequest_ShouldReturnSuccessAndUpdateHotelStarRating(double? baseStarRating, double newStarRating, double resultStarRating)
    {
        // Arrange
        var command = _fixture.Create<PostReviewCommand>();
        var review = _fixture.Build<Review>()
            .With(r => r.StarRating, newStarRating)
            .With(r => r.HotelId, command.HotelId)
            .Create();

        var hotel = _fixture.Build<Hotel>()
            .With(h => h.Id, command.HotelId)
            .With(h => h.StarRating, baseStarRating)
            .Create();

        var reviewResponse = _fixture.Create<ReviewResponse>();

        _hotelRepository.Setup(r => r.IsExistsByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _hotelRepository.Setup(r => r.GetByIdAsync(command.HotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);

        _mapper.Setup(m => m.Map<Review>(command)).Returns(review);

        _reviewRepository.Setup(r => r.IsExistAsync(review, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _reviewRepository.Setup(r => r.AddAsync(review, It.IsAny<CancellationToken>()))
            .ReturnsAsync(review.Id);

        _reviewRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapper.Setup(m => m.Map<ReviewResponse>(review)).Returns(reviewResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(reviewResponse);
        hotel.StarRating.Should().BeApproximately(resultStarRating, 0.001); // check updated value
        _reviewRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Payment.CreatePaymentIntent;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Interfaces;

namespace Application.Tests.Payment;

public class CreatePaymentIntentHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Booking>> _bookingRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IPaymentService> _paymentService;
    private readonly CreatePaymentIntentCommandHandler _handler;

    public CreatePaymentIntentHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _bookingRepository = new Mock<IRepository<Booking>>();
        _userRepository = new Mock<IUserRepository>();
        _paymentService = new Mock<IPaymentService>();

        _handler = new CreatePaymentIntentCommandHandler(
            _paymentService.Object,
            _bookingRepository.Object,
            _userRepository.Object
        );
    }
    
    [Fact]
    public async Task CreatePaymentIntentHandler_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<CreatePaymentIntentCommand>();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, command.BookingId)
            .With(b => b.UserId, command.UserId)
            .With(b => b.Status, BookingStatus.Pending)
            .Create();
        var user = _fixture.Build<User>().With(u => u.Id, command.UserId).Create();
        var clientSecret = _fixture.Create<string>();

        _bookingRepository.Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _userRepository.Setup(r => r.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _paymentService.Setup(p => p.CreatePaymentIntentAsync(
                booking.TotalCost,
                "usd",
                user.Email,
                booking.Id.ToString(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientSecret);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ClientSecret.Should().Be(clientSecret);
    }

    [Fact]
    public async Task CreatePaymentIntentHandler_BookingNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreatePaymentIntentCommand>();

        _bookingRepository.Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.BookingNotFound());
    }

    [Fact]
    public async Task CreatePaymentIntentHandler_UserNotOwner_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreatePaymentIntentCommand>();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, command.BookingId)
            .With(b => b.UserId, Guid.NewGuid())
            .With(b => b.Status, BookingStatus.Pending)
            .Create();

        _bookingRepository.Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.UserNotOwnerOfBooking());
    }

    [Fact]
    public async Task CreatePaymentIntentHandler_BookingNotPending_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CreatePaymentIntentCommand>();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, command.BookingId)
            .With(b => b.UserId, command.UserId)
            .With(b => b.Status, BookingStatus.Confirmed)
            .Create();

        _bookingRepository.Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.BookingCannotBeConfirmed());
    }
}
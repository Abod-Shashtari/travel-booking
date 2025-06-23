using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Bookings.ConfirmBooking;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Bookings;

public class ConfirmBookingCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Booking>> _bookingRepository;
    private readonly ConfirmBookingCommandHandler _handler;

    public ConfirmBookingCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _bookingRepository = new Mock<IRepository<Booking>>();
        _handler = new ConfirmBookingCommandHandler(_bookingRepository.Object);
    }

    [Fact]
    public async Task ConfirmBookingCommandHandler_BookingNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<ConfirmBookingCommand>();
        _bookingRepository.Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.BookingNotFound());
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ConfirmBookingCommandHandler_UserNotOwner_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<ConfirmBookingCommand>();
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
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ConfirmBookingCommandHandler_InvalidStatus_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<ConfirmBookingCommand>();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, command.BookingId)
            .With(b => b.UserId, command.UserId)
            .With(b => b.Status, BookingStatus.Cancelled)
            .Create();

        _bookingRepository.Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.BookingCannotBeConfirmed());
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ConfirmBookingCommandHandler_ValidRequest_ShouldConfirmBookingAndReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<ConfirmBookingCommand>();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, command.BookingId)
            .With(b => b.UserId, command.UserId)
            .With(b => b.Status, BookingStatus.Pending)
            .Create();

        _bookingRepository.Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _bookingRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Confirmed);
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
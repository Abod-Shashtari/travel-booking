using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Bookings.CompleteBooking;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Bookings;

public class CompleteBookingCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Booking>> _bookingRepository;
    private readonly CompleteBookingCommandHandler _handler;

    public CompleteBookingCommandHandlerTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _bookingRepository = new Mock<IRepository<Booking>>();
        _handler = new CompleteBookingCommandHandler(_bookingRepository.Object);
    }

    [Fact]
    public async Task CompleteBookingCommandHandler_BookingNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<CompleteBookingCommand>();

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
    public async Task CompleteBookingCommandHandler_BookingNotPending_ShouldReturnBookingCannotBeCompleted()
    {
        // Arrange
        var command = _fixture.Create<CompleteBookingCommand>();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, command.BookingId)
            .With(b => b.Status, BookingStatus.Pending)
            .Create();

        _bookingRepository.Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.BookingCannotBeCompleted());
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CompleteBookingCommandHandler_ValidRequest_ShouldMarkBookingAsCompletedAndReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<CompleteBookingCommand>();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, command.BookingId)
            .With(b => b.Status, BookingStatus.Confirmed)
            .Create();

        _bookingRepository.Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _bookingRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Completed);
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
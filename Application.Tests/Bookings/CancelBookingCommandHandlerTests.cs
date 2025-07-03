using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Bookings.CancelBooking;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Bookings;

public class CancelBookingCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Booking>> _bookingRepository;
    private readonly CancelBookingCommandHandler _handler;

    public CancelBookingCommandHandlerTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization { ConfigureMembers = true });
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _bookingRepository = new Mock<IRepository<Booking>>();
        _handler = new CancelBookingCommandHandler(_bookingRepository.Object);
    }

    [Fact]
    public async Task CancelBookingCommandHandler_BookingDoesNotExist_ShouldReturnBookingNotFound()
    {
        // Arrange
        var command = _fixture.Create<CancelBookingCommand>();
        _bookingRepository
            .Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.BookingNotFound());
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelBookingCommandHandler_UserIsNotOwner_ShouldReturnUserNotOwnerOfBooking()
    {
        // Arrange
        var command = _fixture.Create<CancelBookingCommand>();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, command.BookingId)
            .With(b => b.UserId, Guid.Empty)
            .Create();

        _bookingRepository
            .Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.UserNotOwnerOfBooking());
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelBookingCommandHandler_BookingNotPending_ShouldReturnBookingUncancellable()
    {
        // Arrange
        var command = _fixture.Create<CancelBookingCommand>();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, command.BookingId)
            .With(b => b.UserId, command.UserId)
            .With(b => b.Status, BookingStatus.Confirmed)
            .Create();

        _bookingRepository
            .Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.BookingUncancellable());
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelBookingCommandHandler_ValidRequest_ShouldCancelAndReturnSuccess()
    {
        // Arrange
        var command = _fixture.Create<CancelBookingCommand>();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, command.BookingId)
            .With(b => b.UserId, command.UserId)
            .With(b => b.Status, BookingStatus.Pending)
            .Create();

        _bookingRepository
            .Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _bookingRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Cancelled);
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
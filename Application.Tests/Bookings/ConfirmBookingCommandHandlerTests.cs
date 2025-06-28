using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Bookings.ConfirmBooking;
using TravelBooking.Application.Bookings.Specifications;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common.Interfaces;

namespace Application.Tests.Bookings;

public class ConfirmBookingCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRepository<Booking>> _bookingRepository;
    private readonly Mock<IBookingConfirmationNumberGenerator> _confirmationGenerator;
    private readonly Mock<IPdfGenerator> _pdfGenerator;
    private readonly Mock<IEmailSender> _emailSender;
    private readonly ConfirmBookingCommandHandler _handler;

    public ConfirmBookingCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _bookingRepository = new Mock<IRepository<Booking>>();
        _confirmationGenerator = new Mock<IBookingConfirmationNumberGenerator>();
        _pdfGenerator = new Mock<IPdfGenerator>();
        _emailSender = new Mock<IEmailSender>();

        _handler = new ConfirmBookingCommandHandler(
            _bookingRepository.Object,
            _confirmationGenerator.Object,
            _emailSender.Object,
            _pdfGenerator.Object
        );
    }

    [Fact]
    public async Task Handle_BookingNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<ConfirmBookingCommand>();
        _bookingRepository
            .Setup(r => r.GetByIdAsync(
                command.BookingId,
                It.IsAny<IncludeNavigationsForABookSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.BookingNotFound());
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidStatus_ShouldReturnFailure()
    {
        // Arrange
        var command = _fixture.Create<ConfirmBookingCommand>();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, command.BookingId)
            .With(b => b.Status, BookingStatus.Cancelled)
            .Create();

        _bookingRepository
            .Setup(r => r.GetByIdAsync(
                command.BookingId,
                It.IsAny<IncludeNavigationsForABookSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookingErrors.BookingCannotBeConfirmed());
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldConfirmAndSendEmail()
    {
        // Arrange
        var command = _fixture.Create<ConfirmBookingCommand>();
        var expectedPdf = _fixture.Create<PdfResult>();
        var confirmation = "BK2025062814083200000000";

        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, command.BookingId)
            .With(b => b.Status, BookingStatus.Pending)
            .Create();

        _bookingRepository
            .Setup(r => r.GetByIdAsync(
                command.BookingId,
                It.IsAny<IncludeNavigationsForABookSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        _confirmationGenerator
            .Setup(g => g.GenerateBookingConfirmationNumber())
            .Returns(confirmation);

        _pdfGenerator
            .Setup(p => p.GeneratePdfAsync(booking, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPdf);

        _emailSender
            .Setup(e => e.SendEmailAsync(
                command.Email,
                It.IsAny<string>(),
                It.Is<string>(body => body.Contains(confirmation)),
                expectedPdf,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _bookingRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Confirmed);
        booking.ConfirmationNumber.Should().Be(confirmation);
        _pdfGenerator.Verify(p => p.GeneratePdfAsync(booking, It.IsAny<CancellationToken>()), Times.Once);
        _emailSender.Verify(e => e.SendEmailAsync(
            command.Email,
            It.IsAny<string>(),
            It.Is<string>(body => body.Contains(confirmation)),
            expectedPdf,
            It.IsAny<CancellationToken>()), Times.Once);
        _bookingRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
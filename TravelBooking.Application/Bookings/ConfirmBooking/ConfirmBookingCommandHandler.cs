using MediatR;
using TravelBooking.Application.Bookings.Specifications;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Bookings.ConfirmBooking;

public class ConfirmBookingCommandHandler:IRequestHandler<ConfirmBookingCommand,Result>
{
    private readonly IRepository<Booking> _bookingRepository;
    private readonly IBookingConfirmationNumberGenerator _bookingConfirmationNumberGenerator;
    private readonly IEmailSender _emailSender;
    private readonly IPdfGenerator _pdfGenerator;
    public ConfirmBookingCommandHandler(IRepository<Booking> bookingRepository, IBookingConfirmationNumberGenerator bookingConfirmationNumberGenerator, IEmailSender emailSender, IPdfGenerator pdfGenerator)
    {
        _bookingRepository = bookingRepository;
        _bookingConfirmationNumberGenerator = bookingConfirmationNumberGenerator;
        _emailSender = emailSender;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<Result> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        var specification = new IncludeNavigationsForABookSpecification();
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, specification, cancellationToken);
        if (booking == null)
            return Result.Failure(BookingErrors.BookingNotFound());

        if(booking.Status != BookingStatus.Pending)
            return Result.Failure(BookingErrors.BookingCannotBeConfirmed());
        
        booking.Status = BookingStatus.Confirmed;
        
        booking.ConfirmationNumber=_bookingConfirmationNumberGenerator.GenerateBookingConfirmationNumber();
        var pdfResult= await _pdfGenerator.GeneratePdfAsync(booking, cancellationToken);
        
        await _emailSender.SendEmailAsync(
            request.Email,
            "Hotel Booking Confirmation",
            GetEmailBody(booking),
            pdfResult,
            cancellationToken
        );
        
        await _bookingRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private string GetEmailBody(Booking booking)
    {
        return $"""
                <h2>Thank you for booking a hotel room in our platform!</h2>
                <p>Your booking has been <strong>confirmed</strong>.</p>
                <p><strong>Confirmation Number:</strong> {booking.ConfirmationNumber}</p>
                <p>We look forward to hosting you!</p>
                <br/>
                <p>The PDf Booking Invoice is attached with the email.</p>
                """;
    }
}
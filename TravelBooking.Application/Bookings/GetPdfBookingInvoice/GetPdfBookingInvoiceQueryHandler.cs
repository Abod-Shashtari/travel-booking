using MediatR;
using TravelBooking.Application.Bookings.Specifications;
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Errors;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;

namespace TravelBooking.Application.Bookings.GetPdfBookingInvoice;

public class GetPdfBookingInvoiceQueryHandler:IRequestHandler<GetPdfBookingInvoiceQuery,Result<PdfResult?>>
{
    private readonly IRepository<Booking> _bookingRepository;
    private readonly IPdfGenerator _pdfGenerator;

    public GetPdfBookingInvoiceQueryHandler(IRepository<Booking> bookingRepository, IPdfGenerator pdfGenerator)
    {
        _bookingRepository = bookingRepository;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<Result<PdfResult?>> Handle(GetPdfBookingInvoiceQuery request, CancellationToken cancellationToken)
    {
        var specification = new IncludeNavigationsForABookSpecification();
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId,specification, cancellationToken);
        if (booking == null) return Result<PdfResult?>.Failure(BookingErrors.BookingNotFound());
        
        if(booking.UserId!=request.UserId) return Result<PdfResult?>.Failure(BookingErrors.UserNotOwnerOfBooking());
        
        var pdfResult= await _pdfGenerator.GeneratePdfAsync(booking,cancellationToken);
        
        return Result<PdfResult?>.Success(pdfResult);
    }
}
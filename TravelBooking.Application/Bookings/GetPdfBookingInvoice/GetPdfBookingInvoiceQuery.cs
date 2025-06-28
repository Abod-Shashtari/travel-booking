using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Bookings.GetPdfBookingInvoice;

public record GetPdfBookingInvoiceQuery(Guid BookingId,Guid UserId):IRequest<Result<PdfResult?>>;
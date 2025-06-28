using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Bookings.Entities;

namespace TravelBooking.Application.Common.Interfaces;

public interface IPdfGenerator
{
    Task<PdfResult> GeneratePdfAsync(Booking booking, CancellationToken cancellationToken=default);
}
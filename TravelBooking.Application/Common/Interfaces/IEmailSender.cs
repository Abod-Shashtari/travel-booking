using TravelBooking.Application.Common.Models;

namespace TravelBooking.Application.Common.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message, PdfResult? pdfResult = null, CancellationToken cancellationToken = default);
}
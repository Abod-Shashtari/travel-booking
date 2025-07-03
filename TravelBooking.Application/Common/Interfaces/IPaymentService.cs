namespace TravelBooking.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<string> CreatePaymentIntentAsync(decimal amount, string currency, string receiptEmail, string bookingId, CancellationToken cancellationToken = default);
}
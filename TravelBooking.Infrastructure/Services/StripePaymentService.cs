using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using Stripe;
using TravelBooking.Application.Common.Interfaces;

namespace TravelBooking.Infrastructure.Services;

[ServiceImplementation]
[RegisterAs<IPaymentService>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class StripePaymentService:IPaymentService
{
    private readonly PaymentIntentService _stripeService;
    
    public StripePaymentService()
    {
        _stripeService = new PaymentIntentService();
    }
    
    public async Task<string> CreatePaymentIntentAsync(decimal amount, string currency, string receiptEmail, string bookingId,
        CancellationToken cancellationToken = default)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = currency,
            ReceiptEmail = receiptEmail,
            Metadata = new Dictionary<string, string> {{"BookingId",bookingId}}
        };

        var intent = await _stripeService.CreateAsync(options, cancellationToken: cancellationToken);
        return intent.ClientSecret;
    }
}
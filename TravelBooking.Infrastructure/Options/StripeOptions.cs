namespace TravelBooking.Infrastructure.Options;

public class StripeOptions
{
    public string SecretKey { get; set; }
    public string WebhookSigningSecret { get; set; }
}
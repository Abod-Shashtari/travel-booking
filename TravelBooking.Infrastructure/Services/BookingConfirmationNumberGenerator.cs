using AttributeBasedRegistration;
using AttributeBasedRegistration.Attributes;
using TravelBooking.Application.Common.Interfaces;

namespace TravelBooking.Infrastructure.Services;

[ServiceImplementation]
[RegisterAs<IBookingConfirmationNumberGenerator>]
[Lifetime(ServiceLifetime.InstancePerLifetimeScope)]
public class BookingConfirmationNumberGenerator : IBookingConfirmationNumberGenerator
{
    public string GenerateBookingConfirmationNumber()
    {
        return "BK"+DateTime.Now.ToString("yyyyMMddHHmmss")+Guid.NewGuid().ToString("N").Substring(0,8);
    }
}
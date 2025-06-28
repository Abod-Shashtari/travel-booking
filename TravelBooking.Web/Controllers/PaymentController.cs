using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Payment.CreatePaymentIntent;
using TravelBooking.Web.Extensions;

namespace TravelBooking.Web.Controllers;

[ApiController]
[Route("api/payment")]
[Authorize]
public class PaymentController:ControllerBase
{
    private readonly ISender _sender;
    
    public PaymentController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("bookings/{bookingId}/create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent(Guid bookingId)
    {
        var command = new CreatePaymentIntentCommand(this.GetUserId(),bookingId);
        var result = await _sender.Send(command);
        return result.Match(Ok,this.HandleFailure);
    }
}
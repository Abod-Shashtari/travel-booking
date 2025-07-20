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

    /// <summary>
    /// Creates a payment intent for a specified booking.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to create a payment intent for.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns the payment intent details if successful.</returns>
    /// <response code="200">Payment intent created successfully.</response>
    /// <response code="400">Invalid request or booking state.</response>
    /// <response code="401">Unauthorized user.</response>
    /// <response code="404">Booking not found.</response>
    /// <response code="409">User not allowed</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPost("bookings/{bookingId}/create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent(Guid bookingId, CancellationToken cancellationToken)
    {
        var command = new CreatePaymentIntentCommand(this.GetUserId(),bookingId);
        var result = await _sender.Send(command,cancellationToken);
        return result.Match(Ok,this.HandleFailure);
    }
}
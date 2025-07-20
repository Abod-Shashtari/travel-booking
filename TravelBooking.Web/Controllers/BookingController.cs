using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using TravelBooking.Application.Bookings.CancelBooking;
using TravelBooking.Application.Bookings.CompleteBooking;
using TravelBooking.Application.Bookings.ConfirmBooking;
using TravelBooking.Application.Bookings.CreateBooking;
using TravelBooking.Application.Bookings.GetBookings;
using TravelBooking.Application.Bookings.GetPdfBookingInvoice;
using TravelBooking.Infrastructure.Options;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.Bookings;

namespace TravelBooking.Web.Controllers;

[ApiController]
[Route("api/user/bookings")]
[Authorize]
public class BookingController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly StripeOptions _stripeOptions;
    
    public BookingController(ISender sender, IMapper mapper, IOptions<StripeOptions> stripeOptions)
    {
        _sender = sender;
        _mapper = mapper;
        _stripeOptions = stripeOptions.Value;
    }

    /// <summary>
    /// Creates a new booking for the authenticated user.
    /// </summary>
    /// <param name="request">The request containing booking details</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>The created booking</returns>
    /// <response code="404">Some entity Not Found</response>
    /// <response code="409">Booking is already exists</response>
    /// <response code="201">Booking created</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateBookingCommand>(request) with {UserId = this.GetUserId()};
        var result = await _sender.Send(command,cancellationToken);
        return result.Match(data=>
                Created($"/api/user/bookings/",data),
            this.HandleFailure);
    }
    
    /// <summary>
    /// Cancels an existing booking by ID for the authenticated user.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to cancel</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content</returns>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">Booking not found</response>
    /// <response code="400">This booking can't be canceled</response>
    /// <response code="403">User is not allowed to cancel</response>
    /// <response code="204">booking canceled</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("{bookingId}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid bookingId, CancellationToken cancellationToken)
    {
        var command = new CancelBookingCommand(this.GetUserId(),bookingId);
        var result = await _sender.Send(command,cancellationToken);
        return result.Match(NoContent, this.HandleFailure);
    }
    
    /// <summary>
    /// Retrieves all bookings made by the authenticated user with optional filtering.
    /// </summary>
    /// <param name="request">pagination options</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A list of user bookings</returns>
    /// <response code="401">Unauthorized access</response>
    /// <response code="200">The bookings returned</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    public async Task<IActionResult> GetBookings([FromQuery] GetBookingsRequest request, CancellationToken cancellationToken)
    {
        var query = _mapper.Map<GetBookingsQuery>(request) with {UserId = this.GetUserId()};
        var result = await _sender.Send(query,cancellationToken);
        return result.Match(Ok, this.HandleFailure);
    }

    /// <summary>
    /// Handles Stripe webhook for confirming successful bookings.
    /// </summary>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content</returns>
    /// <remarks>This endpoint is used by Stripe for successful charge.</remarks>
    /// <response code="200">Webhook processed successfully</response>
    /// <response code="400">Something wrong with webhook</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("confirm/stripe-webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmBooking(CancellationToken cancellationToken)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(cancellationToken);
        var signatureHeader = Request.Headers["Stripe-Signature"];
        var stripeEvent = EventUtility.ConstructEvent(json,signatureHeader, _stripeOptions.WebhookSigningSecret);
        if (stripeEvent == null || stripeEvent.Type != EventTypes.ChargeSucceeded) return Ok();

        var charge = stripeEvent.Data.Object as Charge;
        
        var amount= charge!.Amount;
        var email= charge.ReceiptEmail;
        var bookingId = new Guid(charge.Metadata["BookingId"]);
        
        var command = new ConfirmBookingCommand(bookingId, email, amount);
        var result = await _sender.Send(command,cancellationToken);
        return result.Match(NoContent, this.HandleFailure);
    }
    
    /// <summary>
    /// Retrieves a PDF invoice for a specific booking.
    /// </summary>
    /// <param name="bookingId">The ID of the booking</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>The PDF invoice file</returns>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">Booking not found</response>
    /// <response code="403">User not allowed to download the pdf</response>
    /// <response code="200">return pdf file</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpGet("{bookingId}/pdf-invoice")]
    public async Task<IActionResult> GetPdfBookingInvoice(Guid bookingId, CancellationToken cancellationToken)
    {
        var query = new GetPdfBookingInvoiceQuery(bookingId,this.GetUserId());
        var result = await _sender.Send(query,cancellationToken);
        return result.Match(
            output=> File(output!.Data,"application/pdf",output.FileName),
            this.HandleFailure
        );
    }
    
    /// <summary>
    /// Marks a booking as completed (Admin only).
    /// </summary>
    /// <param name="bookingId">The ID of the booking to complete</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content</returns>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">Booking not found</response>
    /// <response code="400">Booking can't set as complete</response>
    /// <response code="204">Booking completed</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("/api/bookings/{bookingId}/complete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CompleteBooking(Guid bookingId, CancellationToken cancellationToken)
    {
        var command = new CompleteBookingCommand(bookingId);
        var result = await _sender.Send(command,cancellationToken);
        return result.Match(NoContent, this.HandleFailure);
    }

}
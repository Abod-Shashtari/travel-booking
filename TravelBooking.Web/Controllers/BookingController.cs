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
using TravelBooking.Application.Common.Interfaces;
using TravelBooking.Infrastructure.Options;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.Bookings;

namespace TravelBooking.Web.Controllers;

[ApiController]
[Route("api/user/bookings")]
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

    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingRequest request)
    {
        var command = _mapper.Map<CreateBookingCommand>(request) with {UserId = this.GetUserId()};
        var result = await _sender.Send(command);
        return result.Match(data=>
                Created($"/api/user/bookings/",data),
            this.HandleFailure);
    }
    
    [HttpPost("{bookingId}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid bookingId)
    {
        var command = new CancelBookingCommand(this.GetUserId(),bookingId);
        var result = await _sender.Send(command);
        return result.Match(NoContent, this.HandleFailure);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBookings([FromQuery] GetBookingsRequest request)
    {
        var query = _mapper.Map<GetBookingsQuery>(request) with {UserId = this.GetUserId()};
        var result = await _sender.Send(query);
        return result.Match(Ok, this.HandleFailure);
    }

    [HttpPost("confirm/stripe-webhook")]
    public async Task<IActionResult> ConfirmBooking()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signatureHeader = Request.Headers["Stripe-Signature"];
        var stripeEvent = EventUtility.ConstructEvent(json,signatureHeader, _stripeOptions.WebhookSigningSecret);
        if (stripeEvent == null || stripeEvent.Type != EventTypes.ChargeSucceeded) return Ok();

        var charge = stripeEvent.Data.Object as Charge;
        
        var amount= charge!.Amount;
        var email= charge.ReceiptEmail;
        var bookingId = new Guid(charge.Metadata["BookingId"]);
        
        var command = new ConfirmBookingCommand(bookingId, email, amount);
        var result = await _sender.Send(command);
        return result.Match(NoContent, this.HandleFailure);
    }
    
    [HttpGet("{bookingId}/pdf-invoice")]
    [Authorize]
    public async Task<IActionResult> GetPdfBookingInvoice(Guid bookingId)
    {
        var query = new GetPdfBookingInvoiceQuery(bookingId,this.GetUserId());
        var result = await _sender.Send(query);
        return result.Match(
            output=> File(output!.Data,"application/pdf",output.FileName),
            this.HandleFailure
        );
    }
    
    [HttpPost("{bookingId}/complete")]
    public async Task<IActionResult> CompleteBooking(Guid bookingId)
    {
        var command = new CompleteBookingCommand(bookingId);
        var result = await _sender.Send(command);
        return result.Match(NoContent, this.HandleFailure);
    }

}
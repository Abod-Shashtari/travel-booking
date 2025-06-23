using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Bookings.CancelBooking;
using TravelBooking.Application.Bookings.CompleteBooking;
using TravelBooking.Application.Bookings.ConfirmBooking;
using TravelBooking.Application.Bookings.CreateBooking;
using TravelBooking.Application.Bookings.GetBookings;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.Bookings;

namespace TravelBooking.Web.Controllers;

[ApiController]
[Route("api/user/bookings")]
public class BookingController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public BookingController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingRequest request)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest("Token does not contain a User ID.");
        
        var command = _mapper.Map<CreateBookingCommand>(request) with {UserId = new Guid(userId.Value)};
        var result = await _sender.Send(command);
        return result.Match(data=>
                Created($"/api/user/bookings/",data),
            this.HandleFailure);
    }
    
    [HttpPost("{bookingId}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid bookingId)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest("Token does not contain a User ID.");
        
        var command = new CancelBookingCommand(new Guid(userId.Value),bookingId);
        var result = await _sender.Send(command);
        return result.Match(NoContent, this.HandleFailure);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBookings([FromQuery] GetBookingsRequest request)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest("Token does not contain a User ID.");
        
        var query = _mapper.Map<GetBookingsQuery>(request) with {UserId = new Guid(userId.Value)};
        var result = await _sender.Send(query);
        return result.Match(Ok, this.HandleFailure);
    }

    [HttpPost("{bookingId}/confirm")]
    public async Task<IActionResult> ConfirmBooking(Guid bookingId)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest("Token does not contain a User ID.");
        
        var command = new ConfirmBookingCommand(new Guid(userId.Value),bookingId);
        var result = await _sender.Send(command);
        return result.Match(NoContent, this.HandleFailure);
    }
    
    [HttpPost("{bookingId}/complete")]
    public async Task<IActionResult> CompleteBooking(Guid bookingId)
    {
        var command = new CompleteBookingCommand(bookingId);
        var result = await _sender.Send(command);
        return result.Match(NoContent, this.HandleFailure);
    }

}
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Hotels.CreateHotel;
using TravelBooking.Application.Hotels.DeleteHotel;
using TravelBooking.Application.Hotels.GetFeaturedHotels;
using TravelBooking.Application.Hotels.GetHotel;
using TravelBooking.Application.Hotels.GetHotels;
using TravelBooking.Application.Hotels.SetThumbnail;
using TravelBooking.Application.Hotels.UpdateHotel;
using TravelBooking.Application.UserActivity.GetRecentlyVisitedHotels;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.Hotels;

namespace TravelBooking.Web.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public HotelController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetHotels([FromQuery] GetHotelsRequest request)
    {
        var query = _mapper.Map<GetHotelsQuery>(request);
        var result = await _sender.Send(query);
        return result.Match(Ok,this.HandleFailure);
    }
    
    [HttpGet("featured-deals")]
    public async Task<IActionResult> GetFeaturedHotels()
    {
        var query = new GetFeaturedHotelsQuery();
        var result = await _sender.Send(query);
        return result.Match(Ok,this.HandleFailure);
    }
    
    [HttpGet("/api/user/recently-visited-hotels")]
    public async Task<IActionResult> GetRecentlyVisitedHotels()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest("Token does not contain a User ID.");
        
        var query = new GetRecentlyVisitedHotelsQuery(new Guid(userId.Value));
        var result = await _sender.Send(query);
        return result.Match(Ok,this.HandleFailure);
    }

    [HttpGet("{hotelId}")]
    public async Task<IActionResult> GetHotel(Guid hotelId)
    {
        var query = new GetHotelQuery(hotelId); 
        var result = await _sender.Send(query);
        return result.Match(Ok,this.HandleFailure);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateHotel(CreateHotelRequest request)
    {
        var command = _mapper.Map<CreateHotelCommand>(request);
        var result = await _sender.Send(command);
        return result.Match(
            createdHotel=>CreatedAtAction(
                nameof(GetHotel),
                new { hotelId = createdHotel!.Id },
                createdHotel
            ),
            this.HandleFailure
        );
    }
    
    [HttpPut("{hotelId}")]
    public async Task<IActionResult> UpdateHotel(Guid hotelId, UpdateHotelRequest request)
    {
        var command = _mapper.Map<UpdateHotelCommand>(request) with { HotelId = hotelId };
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    [HttpDelete("{hotelId}")]
    public async Task<IActionResult> DeleteHotel(Guid hotelId)
    {
        var command = new DeleteHotelCommand(hotelId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
        
    [HttpPut("{hotelId}/thumbnail")]
    public async Task<IActionResult> SetThumbnail(Guid hotelId,[FromBody] Guid imageId)
    {
        var command = new SetHotelThumbnailCommand(hotelId, imageId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }

}
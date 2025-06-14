using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Hotels.CreateHotels;
using TravelBooking.Application.Hotels.DeleteHotel;
using TravelBooking.Application.Hotels.GetHotel;
using TravelBooking.Application.Hotels.GetHotels;
using TravelBooking.Application.Hotels.UpdateHotel;
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
        return result.Match(data => Ok(data),this.HandleFailure);
    }

    [HttpGet("{hotelId}")]
    public async Task<IActionResult> GetHotel(Guid hotelId)
    {
        var query = new GetHotelQuery(hotelId); 
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
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
}
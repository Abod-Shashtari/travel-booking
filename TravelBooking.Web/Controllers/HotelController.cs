using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Hotels.CreateHotels;
using TravelBooking.Application.Hotels.SearchHotel;
using TravelBooking.Web.Extensions;

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

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateHotel(CreateHotelCommand command)
    {
        var result = await _sender.Send(command);
        return result.Match(_=>Created(),this.HandleFailure);
    }

    [HttpGet]
    public async Task<IActionResult> GetHotels([FromQuery] SearchHotelRequest request)
    {
        var query = _mapper.Map<SearchHotelQuery>(request);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
}
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Hotels.CreateHotels;
using TravelBooking.Web.Extensions;

namespace TravelBooking.Web.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelController:ControllerBase
{
    private readonly ISender _sender;

    public HotelController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateHotel(CreateHotelCommand command)
    {
        var result = await _sender.Send(command);
        return result.Match(_=>Created(),this.HandleFailure);
    }

}
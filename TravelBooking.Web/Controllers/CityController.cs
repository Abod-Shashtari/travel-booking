using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Cities.CreateCity;
using TravelBooking.Web.Extensions;

namespace TravelBooking.Web.Controllers;

[Route("api/cities")]
[ApiController]
public class CityController:ControllerBase
{
    private readonly ISender _sender;
    public CityController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateCityCommand command)
    {
        var result = await _sender.Send(command);
        return result.Match(_=>Created(),this.HandleFailure);
    }
}
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Cities.CreateCity;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.Cities;

namespace TravelBooking.Web.Controllers;

[Route("api/cities")]
[ApiController]
public class CityController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    public CityController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateCityRequest request)
    {
        var command = _mapper.Map<CreateCityCommand>(request);
        var result = await _sender.Send(command);
        return result.Match(_=>Created(),this.HandleFailure);
    }
}
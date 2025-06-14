using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Cities.CreateCity;
using TravelBooking.Application.Cities.DeleteCity;
using TravelBooking.Application.Cities.GetCities;
using TravelBooking.Application.Cities.GetCity;
using TravelBooking.Application.Cities.UpdateCity;
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

    [HttpGet]
    public async Task<IActionResult> GetCities([FromQuery] GetCitiesRequest request)
    {
        var query = _mapper.Map<GetCitiesQuery>(request);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }

    [HttpGet("{cityId}")]
    public async Task<IActionResult> GetCity(Guid cityId)
    {
        var query = new GetCityQuery(cityId); 
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCity(CreateCityRequest request)
    {
        var command = _mapper.Map<CreateCityCommand>(request);
        var result = await _sender.Send(command);
        return result.Match(
            createdCity=>CreatedAtAction(
                nameof(GetCity),
                new { cityId = createdCity!.Id },
                createdCity
            ),
            this.HandleFailure
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{cityId}")]
    public async Task<IActionResult> UpdateCity(Guid cityId,UpdateCityRequest request)
    {
        var command = _mapper.Map<UpdateCityRequest, UpdateCityCommand>(request) with { CityId = cityId };
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    [HttpDelete("{cityId}")]
    public async Task<IActionResult> DeleteCity(Guid cityId)
    {
        var command = new DeleteCityCommand(cityId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
}
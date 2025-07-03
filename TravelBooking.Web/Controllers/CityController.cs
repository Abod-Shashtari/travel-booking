using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Cities.CreateCity;
using TravelBooking.Application.Cities.DeleteCity;
using TravelBooking.Application.Cities.GetCities;
using TravelBooking.Application.Cities.GetCity;
using TravelBooking.Application.Cities.GetTrendingCities;
using TravelBooking.Application.Cities.SetThumbnail;
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

    /// <summary>
    /// Retrieves a paginated list of all cities.
    /// </summary>
    /// <param name="request">The request contains the pagination</param>
    /// <returns>A list of cities</returns>
    /// <response code="200">Returns the list of cities</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCities([FromQuery] GetCitiesRequest request)
    {
        var query = _mapper.Map<GetCitiesQuery>(request);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
    
    /// <summary>
    /// Retrieves a list of trending destination cities.
    /// </summary>
    /// <returns>A list of trending cities</returns>
    /// <response code="200">Returns the trending cities</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("trending-destinations")]
    public async Task<IActionResult> GetTrendingCities()
    {
        var query = new GetTrendingCitiesQuery(); 
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }

    /// <summary>
    /// Retrieves details of a specific city by its ID.
    /// </summary>
    /// <param name="cityId">The ID of the city</param>
    /// <returns>The city details</returns>
    /// <response code="200">Returns the city data</response>
    /// <response code="404">City not found</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("{cityId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCity(Guid cityId)
    {
        var query = new GetCityQuery(cityId); 
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
    
    /// <summary>
    /// Creates a new city.
    /// </summary>
    /// <param name="request">The request containing city creation data</param>
    /// <returns>The created city</returns>
    /// <response code="201">City created successfully</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="400">Invalid request data</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Updates an existing city.
    /// </summary>
    /// <param name="cityId">The ID of the city to update</param>
    /// <param name="request">The request containing updated city data</param>
    /// <returns>No content</returns>
    /// <response code="204">City updated successfully</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">City not found</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    [HttpPut("{cityId}")]
    public async Task<IActionResult> UpdateCity(Guid cityId,UpdateCityRequest request)
    {
        var command = _mapper.Map<UpdateCityRequest, UpdateCityCommand>(request) with { CityId = cityId };
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    /// <summary>
    /// Deletes a specific city.
    /// </summary>
    /// <param name="cityId">The ID of the city to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">City deleted successfully</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">City not found</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{cityId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCity(Guid cityId)
    {
        var command = new DeleteCityCommand(cityId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    /// <summary>
    /// Sets the thumbnail image for a specific city.
    /// </summary>
    /// <param name="cityId">The ID of the city</param>
    /// <param name="imageId">The ID of the image to set as the thumbnail</param>
    /// <returns>No content</returns>
    /// <response code="204">Thumbnail updated successfully</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">City or image not found</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{cityId}/thumbnail")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetThumbnail(Guid cityId,[FromBody] Guid imageId)
    {
        var command = new SetCityThumbnailCommand(cityId, imageId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
}
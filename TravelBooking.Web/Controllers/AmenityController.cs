using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Amenities.AddAmenityToRoomType;
using TravelBooking.Application.Amenities.CreateAmenity;
using TravelBooking.Application.Amenities.DeleteAmenity;
using TravelBooking.Application.Amenities.GetAmenities;
using TravelBooking.Application.Amenities.GetAmenitiesByRoomType;
using TravelBooking.Application.Amenities.GetAmenity;
using TravelBooking.Application.Amenities.RemoveAmenityFromRoomType;
using TravelBooking.Application.Amenities.UpdateAmenity;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.Amenities;

namespace TravelBooking.Web.Controllers;

[ApiController]
[Route("api/amenities")]
public class AmenityController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public AmenityController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Retrieves a paginated list of all amenities.
    /// </summary>
    /// <param name="request">Pagination options for the amenity list</param>
    /// <returns>A list of amenities</returns>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAmenities([FromQuery] GetAmenitiesRequest request)
    {
        var query = _mapper.Map<GetAmenitiesQuery>(request);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
    
    /// <summary>
    /// Retrieves all amenities associated with a specific room type.
    /// </summary>
    /// <param name="roomTypeId">The ID of the room type</param>
    /// <returns>List of amenities for the specified room type</returns>
    /// <response code="404">Room type not found</response>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("/api/room-types/{roomTypeId}/amenities")]
    public async Task<IActionResult> GetAmenitiesByRoomType(Guid roomTypeId)
    {
        var query = new GetAmenitiesByRoomTypeQuery(roomTypeId); 
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }

    /// <summary>
    /// Retrieves details of a specific amenity by its ID.
    /// </summary>
    /// <param name="amenityId">The ID of the amenity</param>
    /// <returns>The requested amenity</returns>
    /// <response code="404">Amenity not found</response>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{amenityId}")]
    public async Task<IActionResult> GetAmenity(Guid amenityId)
    {
        var query = new GetAmenityQuery(amenityId); 
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
    
    /// <summary>
    /// Creates a new amenity.
    /// </summary>
    /// <param name="request">The request containing amenity data</param>
    /// <returns>The created amenity</returns>
    /// <response code="401">Unauthorized access</response>
    /// <response code="409">Amenity is already exists</response>
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAmenity(CreateAmenityRequest request)
    {
        var command = _mapper.Map<CreateAmenityCommand>(request);
        var result = await _sender.Send(command);
        return result.Match(
            createdAmenity=>CreatedAtAction(
                nameof(GetAmenity),
                new { amenityId = createdAmenity!.Id },
                createdAmenity
            ),
            this.HandleFailure
        );
    }
    
    /// <summary>
    /// Adds an existing amenity to a specific room type.
    /// </summary>
    /// <param name="roomTypeId">The ID of the room type</param>
    /// <param name="request">The request containing the amenity ID</param>
    /// <returns>No content</returns>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">Room type or amenity Not found</response>
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("/api/room-types/{roomTypeId}/amenities")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddAmenity(Guid roomTypeId, AddAmenityToRoomTypeRequest request)
    {
        var command = new AddAmenityToRoomTypeCommand(roomTypeId, request.AmenityId); 
        var result = await _sender.Send(command);
        return result.Match(NoContent, this.HandleFailure);
    }

    /// <summary>
    /// Updates an existing amenity.
    /// </summary>
    /// <param name="amenityId">The ID of the amenity</param>
    /// <param name="request">The request containing updated amenity data</param>
    /// <returns>No content</returns>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">amenity Not found</response>
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    [HttpPut("{amenityId}")]
    public async Task<IActionResult> UpdateAmenity(Guid amenityId,UpdateAmenityRequest request)
    {
        var command = _mapper.Map<UpdateAmenityRequest, UpdateAmenityCommand>(request) with { AmenityId = amenityId };
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    
    /// <summary>
    /// Deletes a specific amenity.
    /// </summary>
    /// <param name="amenityId">The ID of the amenity to delete</param>
    /// <returns>No content</returns>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">amenity Not found</response>
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{amenityId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAmenity(Guid amenityId)
    {
        var command = new DeleteAmenityCommand(amenityId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    /// <summary>
    /// Removes a specific amenity from a specific room type.
    /// </summary>
    /// <param name="roomTypeId">The ID of the room type</param>
    /// <param name="amenityId">The ID of the amenity to remove</param>
    /// <returns>No content</returns>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">Room type or amenity Not found</response>
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("/api/room-types/{roomTypeId}/amenities/{amenityId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveAmenityFromRoomType(Guid roomTypeId,Guid amenityId)
    {
        var command = new RemoveAmenityFromRoomTypeCommand(roomTypeId, amenityId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
}
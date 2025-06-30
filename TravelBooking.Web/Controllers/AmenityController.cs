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
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAmenities([FromQuery] GetAmenitiesRequest request)
    {
        var query = _mapper.Map<GetAmenitiesQuery>(request);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
    
    [HttpGet("/api/room-types/{roomTypeId}/amenities")]
    public async Task<IActionResult> GetAmenitiesByRoomType(Guid roomTypeId)
    {
        var query = new GetAmenitiesByRoomTypeQuery(roomTypeId); 
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }

    [HttpGet("{amenityId}")]
    public async Task<IActionResult> GetAmenity(Guid amenityId)
    {
        var query = new GetAmenityQuery(amenityId); 
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
    
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
    
    [HttpPost("/api/room-types/{roomTypeId}/amenities")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddAmenity(Guid roomTypeId, AddAmenityToRoomTypeRequest request)
    {
        var command = new AddAmenityToRoomTypeCommand(roomTypeId, request.AmenityId); 
        var result = await _sender.Send(command);
        return result.Match(NoContent, this.HandleFailure);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{amenityId}")]
    public async Task<IActionResult> UpdateAmenity(Guid amenityId,UpdateAmenityRequest request)
    {
        var command = _mapper.Map<UpdateAmenityRequest, UpdateAmenityCommand>(request) with { AmenityId = amenityId };
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    [HttpDelete("{amenityId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAmenity(Guid amenityId)
    {
        var command = new DeleteAmenityCommand(amenityId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    [HttpDelete("/api/room-types/{roomTypeId}/amenities/{amenityId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveAmenityFromRoomType(Guid roomTypeId,Guid amenityId)
    {
        var command = new RemoveAmenityFromRoomTypeCommand(roomTypeId, amenityId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
}
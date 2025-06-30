using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.RoomTypes.CreateRoomType;
using TravelBooking.Application.RoomTypes.DeleteRoomType;
using TravelBooking.Application.RoomTypes.GetRoomType;
using TravelBooking.Application.RoomTypes.GetRoomTypes;
using TravelBooking.Application.RoomTypes.UpdateRoomType;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.RoomTypes;

namespace TravelBooking.Web.Controllers;

[Route("api/room-types")]
[ApiController]
public class RoomTypeController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public RoomTypeController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRoomTypes([FromQuery] GetRoomTypesRequest request)
    {
        var query = _mapper.Map<GetRoomTypesQuery>(request);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
    
    [HttpGet("{roomTypeId}")]
    public async Task<IActionResult> GetRoomType(Guid roomTypeId)
    {
        var query = new GetRoomTypeQuery(roomTypeId); 
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTypeRoom(CreateRoomTypeRequest request)
    {
        var command = _mapper.Map<CreateRoomTypeCommand>(request);
        var result = await _sender.Send(command);
        return result.Match(
            createdRoomType=>CreatedAtAction(
                nameof(GetRoomType),
                new { roomTypeId = createdRoomType!.Id },
                createdRoomType
            ),
            this.HandleFailure
        );
    }
    
    [HttpPut("{roomTypeId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTypeRoom(Guid roomTypeId, UpdateRoomTypeRequest request)
    {
        var command = _mapper.Map<UpdateRoomTypeCommand>(request) with { RoomTypeId = roomTypeId };
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    [HttpDelete("{roomTypeId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTypeRoom(Guid roomTypeId)
    {
        var command = new DeleteRoomTypeCommand(roomTypeId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
}
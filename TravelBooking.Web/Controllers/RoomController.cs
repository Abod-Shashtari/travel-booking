using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Rooms.CreateRoom;
using TravelBooking.Application.Rooms.DeleteRoom;
using TravelBooking.Application.Rooms.GetRoom;
using TravelBooking.Application.Rooms.GetRooms;
using TravelBooking.Application.Rooms.UpdateRoom;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.Rooms;

namespace TravelBooking.Web.Controllers;

[Route("api/rooms")]
[ApiController]
public class RoomController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public RoomController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }
    
    [HttpGet("/api/room-types/{roomTypeId}/rooms")]
    public async Task<IActionResult> GetRooms(Guid roomTypeId,[FromQuery] GetRoomsRequest request)
    {
        var query = _mapper.Map<GetRoomsQuery>(request) with {RoomTypeId = roomTypeId};
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }

    [HttpGet("{roomId}")]
    public async Task<IActionResult> GetRoom(Guid roomId)
    {
        var query = new GetRoomQuery(roomId);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
    
    [HttpPost("/api/room-types/{roomTypeId}/rooms")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRoom(Guid roomTypeId,CreateRoomRequest request)
    {
        var command = _mapper.Map<CreateRoomCommand>(request) with {RoomTypeId = roomTypeId};
        var result = await _sender.Send(command);
        return result.Match(
            createdRoom=>CreatedAtAction(
                nameof(GetRoom),
                new { roomId = createdRoom!.Id },
                createdRoom
            ),
            this.HandleFailure
        );
    }

    [HttpPut("{roomId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRoom(Guid roomId,UpdateRoomRequest request)
    {
        var command = _mapper.Map<UpdateRoomCommand>(request) with { RoomId = roomId };
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    [HttpDelete("{roomId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRoom(Guid roomId)
    {
        var command = new DeleteRoomCommand(roomId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
}
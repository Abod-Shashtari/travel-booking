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
    
    /// <summary>
    /// Retrieves a list of rooms for a specific room type.
    /// </summary>
    /// <param name="roomTypeId">The ID of the room type.</param>
    /// <param name="request">The request contains pagination options</param>
    /// <returns>A list of rooms.</returns>
    /// <response code="200">Returns the list of rooms.</response>
    /// <response code="404">Room type not found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("/api/room-types/{roomTypeId}/rooms")]
    public async Task<IActionResult> GetRooms(Guid roomTypeId,[FromQuery] GetRoomsRequest request)
    {
        var query = _mapper.Map<GetRoomsQuery>(request) with {RoomTypeId = roomTypeId};
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }

    /// <summary>
    /// Retrieves details of a specific room.
    /// </summary>
    /// <param name="roomId">The ID of the room.</param>
    /// <returns>Room details.</returns>
    /// <response code="200">Returns the room details.</response>
    /// <response code="404">Room not found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{roomId}")]
    public async Task<IActionResult> GetRoom(Guid roomId)
    {
        var query = new GetRoomQuery(roomId);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
    
    /// <summary>
    /// Creates a new room under a specific room type.
    /// </summary>
    /// <param name="roomTypeId">The ID of the room type.</param>
    /// <param name="request">Room creation data.</param>
    /// <returns>The created room.</returns>
    /// <response code="201">Room successfully created.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    /// <response code="404">Room type not found.</response>
    /// <response code="409">Room already exists.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    /// <summary>
    /// Updates an existing room.
    /// </summary>
    /// <param name="roomId">The ID of the room to update.</param>
    /// <param name="request">The updated room data.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Room successfully updated.</response>
    /// <response code="400">Invalid data provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    /// <response code="404">Room not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{roomId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRoom(Guid roomId,UpdateRoomRequest request)
    {
        var command = _mapper.Map<UpdateRoomCommand>(request) with { RoomId = roomId };
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    /// <summary>
    /// Deletes a specific room.
    /// </summary>
    /// <param name="roomId">The ID of the room to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Room successfully deleted.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    /// <response code="404">Room not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{roomId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRoom(Guid roomId)
    {
        var command = new DeleteRoomCommand(roomId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
}
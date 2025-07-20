using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.RoomTypes.CreateRoomType;
using TravelBooking.Application.RoomTypes.DeleteRoomType;
using TravelBooking.Application.RoomTypes.GetRoomType;
using TravelBooking.Application.RoomTypes.GetRoomTypes;
using TravelBooking.Application.RoomTypes.GetRoomTypesOfHotel;
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

    /// <summary>
    /// Retrieves a list of all room types.
    /// </summary>
    /// <param name="request">The request contains the pagination</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A list of room types.</returns>
    /// <response code="200">Returns the list of room types.</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRoomTypes([FromQuery] GetRoomTypesRequest request, CancellationToken cancellationToken)
    {
        var query = _mapper.Map<GetRoomTypesQuery>(request);
        var result = await _sender.Send(query,cancellationToken);
        return result.Match(Ok,this.HandleFailure);
    }
    
    /// <summary>
    /// Retrieves a list of room types of hotel.
    /// </summary>
    /// <param name="request">The request contains the pagination</param>
    /// <param name="hotelId">The hotel to return its room types</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A list of room types.</returns>
    /// <response code="200">Returns list of room types for specific hotel.</response>
    /// <response code="404">hotel is not found</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("/api/hotels/{hotelId}/room-types")]
    public async Task<IActionResult> GetRoomTypesOfHotel(Guid hotelId, [FromQuery] GetRoomTypesOfHotelRequest request, CancellationToken cancellationToken)
    {
        var query = _mapper.Map<GetRoomTypesOfHotelQuery>(request) with {HotelId = hotelId};
        var result = await _sender.Send(query,cancellationToken);
        return result.Match(Ok,this.HandleFailure);
    }
    
    /// <summary>
    /// Retrieves a specific room type by ID.
    /// </summary>
    /// <param name="roomTypeId">The ID of the room type.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>The requested room type.</returns>
    /// <response code="200">Returns the room type.</response>
    /// <response code="404">Room type not found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{roomTypeId}")]
    public async Task<IActionResult> GetRoomType(Guid roomTypeId, CancellationToken cancellationToken)
    {
        var query = new GetRoomTypeQuery(roomTypeId); 
        var result = await _sender.Send(query,cancellationToken);
        return result.Match(Ok,this.HandleFailure);
    }

    /// <summary>
    /// Creates a new room type.
    /// </summary>
    /// <param name="request">The details of the room type to create.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>The created room type.</returns>
    /// <response code="201">Room type successfully created.</response>
    /// <response code="400">Invalid input data.</response>
    /// <response code="409">Room type already exists.</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTypeRoom(CreateRoomTypeRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateRoomTypeCommand>(request);
        var result = await _sender.Send(command,cancellationToken);
        return result.Match(
            createdRoomType=>CreatedAtAction(
                nameof(GetRoomType),
                new { roomTypeId = createdRoomType!.Id },
                createdRoomType
            ),
            this.HandleFailure
        );
    }
    
    /// <summary>
    /// Updates an existing room type.
    /// </summary>
    /// <param name="roomTypeId">The ID of the room type to update.</param>
    /// <param name="request">The updated room type data.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Room type successfully updated.</response>
    /// <response code="400">Invalid input data.</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">Room type not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{roomTypeId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTypeRoom(Guid roomTypeId, UpdateRoomTypeRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateRoomTypeCommand>(request) with { RoomTypeId = roomTypeId };
        var result = await _sender.Send(command,cancellationToken);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    /// <summary>
    /// Deletes a specific room type.
    /// </summary>
    /// <param name="roomTypeId">The ID of the room type to delete.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Room type successfully deleted.</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">Room type not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{roomTypeId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTypeRoom(Guid roomTypeId, CancellationToken cancellationToken)
    {
        var command = new DeleteRoomTypeCommand(roomTypeId);
        var result = await _sender.Send(command,cancellationToken);
        return result.Match(NoContent,this.HandleFailure);
    }
}
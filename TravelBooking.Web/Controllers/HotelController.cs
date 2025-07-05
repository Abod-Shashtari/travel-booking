using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Hotels.CreateHotel;
using TravelBooking.Application.Hotels.DeleteHotel;
using TravelBooking.Application.Hotels.GetFeaturedHotels;
using TravelBooking.Application.Hotels.GetHotel;
using TravelBooking.Application.Hotels.GetHotels;
using TravelBooking.Application.Hotels.SearchHotels;
using TravelBooking.Application.Hotels.SetThumbnail;
using TravelBooking.Application.Hotels.UpdateHotel;
using TravelBooking.Application.UserActivity.GetRecentlyVisitedHotels;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.Hotels;
using TravelBooking.Web.Requests.Images;

namespace TravelBooking.Web.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public HotelController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    /// <summary>
    /// Searches for hotels based on filters like city, dates, price, etc.
    /// </summary>
    /// <param name="request">Search criteria for hotels</param>
    /// <returns>List of hotels matching the criteria</returns>
    /// <response code="200">Hotels found successfully</response>
    /// <response code="400">Invalid search parameters</response>
    /// <response code="409">hotel already exists</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpGet("search")]
    public async Task<IActionResult> SearchHotels([FromQuery] SearchHotelsRequest request)
    {
        var query = _mapper.Map<SearchHotelsQuery>(request);
        var result = await _sender.Send(query);
        return result.Match(Ok,this.HandleFailure);
    }
    
    /// <summary>
    /// Retrieves a list of hotels (Admin only).
    /// </summary>
    /// <param name="request">The request contains pagination options</param>
    /// <returns>Paginated list of hotels</returns>
    /// <response code="200">Hotels retrieved</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetHotels([FromQuery] GetHotelsRequest request)
    {
        var query = _mapper.Map<GetHotelsQuery>(request);
        var result = await _sender.Send(query);
        return result.Match(Ok,this.HandleFailure);
    }
    
    /// <summary>
    /// Gets a list of featured hotel deals.
    /// </summary>
    /// <returns>List of featured hotel deals</returns>
    /// <response code="200">Featured deals returned</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("featured-deals")]
    public async Task<IActionResult> GetFeaturedHotels()
    {
        var query = new GetFeaturedHotelsQuery();
        var result = await _sender.Send(query);
        return result.Match(Ok,this.HandleFailure);
    }
    
    /// <summary>
    /// Retrieves the user's recently visited hotels.
    /// </summary>
    /// <returns>List of recently visited hotels</returns>
    /// <response code="200">Recently visited hotels returned</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("/api/user/recently-visited-hotels")]
    [Authorize]
    public async Task<IActionResult> GetRecentlyVisitedHotels()
    {
        var query = new GetRecentlyVisitedHotelsQuery(this.GetUserId());
        var result = await _sender.Send(query);
        return result.Match(Ok,this.HandleFailure);
    }

    /// <summary>
    /// Retrieves the details of a specific hotel.
    /// </summary>
    /// <param name="hotelId">The ID of the hotel</param>
    /// <returns>Hotel details</returns>
    /// <response code="200">Hotel retrieved successfully</response>
    /// <response code="404">Hotel not found</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{hotelId}")]
    public async Task<IActionResult> GetHotel(Guid hotelId)
    {
        var query = new GetHotelQuery(this.GetUserId(),hotelId); 
        var result = await _sender.Send(query);
        return result.Match(Ok,this.HandleFailure);
    }

    /// <summary>
    /// Creates a new hotel (Admin only).
    /// </summary>
    /// <param name="request">The request containing hotel details</param>
    /// <returns>The created hotel</returns>
    /// <response code="201">Hotel created successfully</response>
    /// <response code="400">Invalid hotel data</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">Some field Not Found</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateHotel(CreateHotelRequest request)
    {
        var command = _mapper.Map<CreateHotelCommand>(request);
        var result = await _sender.Send(command);
        return result.Match(
            createdHotel=>CreatedAtAction(
                nameof(GetHotel),
                new { hotelId = createdHotel!.Id },
                createdHotel
            ),
            this.HandleFailure
        );
    }
    
    /// <summary>
    /// Updates an existing hotel (Admin only).
    /// </summary>
    /// <param name="hotelId">The ID of the hotel</param>
    /// <param name="request">The request containing updated hotel info</param>
    /// <returns>No content</returns>
    /// <response code="204">Hotel updated successfully</response>
    /// <response code="400">Invalid update data</response>
    /// <response code="404">Hotel or other fields not found</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{hotelId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateHotel(Guid hotelId, UpdateHotelRequest request)
    {
        var command = _mapper.Map<UpdateHotelCommand>(request) with { HotelId = hotelId };
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    /// <summary>
    /// Deletes a specific hotel (Admin only).
    /// </summary>
    /// <param name="hotelId">The ID of the hotel</param>
    /// <returns>No content</returns>
    /// <response code="204">Hotel deleted</response>
    /// <response code="404">Hotel not found</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete("{hotelId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteHotel(Guid hotelId)
    {
        var command = new DeleteHotelCommand(hotelId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
        
    /// <summary>
    /// Sets the thumbnail image for a hotel (Admin only).
    /// </summary>
    /// <param name="hotelId">The ID of the hotel</param>
    /// <param name="request">The ID of the image to set as thumbnail</param>
    /// <returns>No content</returns>
    /// <response code="204">Thumbnail set successfully</response>
    /// <response code="404">Hotel or image not found</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut("{hotelId}/thumbnail")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetThumbnail(Guid hotelId, SetThumbnailImageRequest request)
    {
        var command = new SetHotelThumbnailCommand(hotelId, request.ImageId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }

}
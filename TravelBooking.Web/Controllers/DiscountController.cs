using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Discounts.CreateDiscount;
using TravelBooking.Application.Discounts.DeleteDiscount;
using TravelBooking.Application.Discounts.GetDiscount;
using TravelBooking.Application.Discounts.GetDiscounts;
using TravelBooking.Application.Discounts.UpdateDiscount;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.Discounts;

namespace TravelBooking.Web.Controllers;

[ApiController]
[Route("api/discounts")]
[Authorize(Roles = "Admin")]
public class DiscountController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    public DiscountController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }
    /// <summary>
    /// Retrieves a paginated list of discounts.
    /// </summary>
    /// <param name="request">The request contains pagination options</param>
    /// <returns>A list of discounts</returns>
    /// <response code="200">Returns the list of discounts</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    public async Task<IActionResult> GetDiscounts([FromQuery] GetDiscountsRequest request)
    {
        var query = _mapper.Map<GetDiscountsQuery>(request);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }

    /// <summary>
    /// Retrieves the details of a specific discount.
    /// </summary>
    /// <param name="discountId">The ID of the discount</param>
    /// <returns>The discount details</returns>
    /// <response code="200">Returns the discount</response>
    /// <response code="404"> not found</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("{discountId}")]
    public async Task<IActionResult> GetDiscount(Guid discountId)
    {
        var query = new GetDiscountQuery(discountId); 
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
    
    
    /// <summary>
    /// Creates a new discount for a specific room type.
    /// </summary>
    /// <param name="roomTypeId">The ID of the room type</param>
    /// <param name="request">The request containing discount details</param>
    /// <returns>The created discount</returns>
    /// <response code="201">Discount created successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="409">discount already exists</response>
    /// <response code="404">Room type not found</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("/api/room-types/{roomTypeId}/discounts")]
    public async Task<IActionResult> CreateDiscount(Guid roomTypeId, CreateDiscountRequest request)
    {
        var command = _mapper.Map<CreateDiscountCommand>(request) with {RoomTypeId = roomTypeId};
        var result = await _sender.Send(command);
        return result.Match(
            createdDiscount=>CreatedAtAction(
                nameof(GetDiscount),
                new { discountId = createdDiscount!.Id },
                createdDiscount
            ),
            this.HandleFailure
        );
    }

    /// <summary>
    /// Updates an existing discount.
    /// </summary>
    /// <param name="discountId">The ID of the discount to update</param>
    /// <param name="request">The request containing updated discount information</param>
    /// <returns>No content</returns>
    /// <response code="204">Discount updated successfully</response>
    /// <response code="400">Invalid update request</response>
    /// <response code="404">Discount not found</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut("{discountId}")]
    public async Task<IActionResult> UpdateDiscount(Guid discountId,UpdateDiscountRequest request)
    {
        var command = _mapper.Map<UpdateDiscountRequest, UpdateDiscountCommand>(request) with { DiscountId = discountId };
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    /// <summary>
    /// Deletes a specific discount.
    /// </summary>
    /// <param name="discountId">The ID of the discount to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">Discount deleted successfully</response>
    /// <response code="404">Discount not found</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete("{discountId}")]
    public async Task<IActionResult> DeleteDiscount(Guid discountId)
    {
        var command = new DeleteDiscountCommand(discountId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
}
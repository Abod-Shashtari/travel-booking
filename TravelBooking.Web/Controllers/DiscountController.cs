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
public class DiscountController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    public DiscountController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDiscounts([FromQuery] GetDiscountsRequest request)
    {
        var query = _mapper.Map<GetDiscountsQuery>(request);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }

    [HttpGet("{discountId}")]
    public async Task<IActionResult> GetDiscount(Guid discountId)
    {
        var query = new GetDiscountQuery(discountId); 
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data),this.HandleFailure);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateDiscount(CreateDiscountRequest request)
    {
        var command = _mapper.Map<CreateDiscountCommand>(request);
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

    [Authorize(Roles = "Admin")]
    [HttpPut("{discountId}")]
    public async Task<IActionResult> UpdateDiscount(Guid discountId,UpdateDiscountRequest request)
    {
        var command = _mapper.Map<UpdateDiscountRequest, UpdateDiscountCommand>(request) with { DiscountId = discountId };
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    [HttpDelete("{discountId}")]
    public async Task<IActionResult> DeleteDiscount(Guid discountId)
    {
        var command = new DeleteDiscountCommand(discountId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
}
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Reviews.DeleteReview;
using TravelBooking.Application.Reviews.GetReviews;
using TravelBooking.Application.Reviews.PostReview;
using TravelBooking.Application.Reviews.UpdateReview;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.Reviews;

namespace TravelBooking.Web.Controllers;

[ApiController]
public class ReviewController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public ReviewController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("/api/hotels/{hotelId}/reviews")]
    public async Task<IActionResult> GetReviews(Guid hotelId,[FromQuery] GetReviewsRequest request)
    {
        var query = _mapper.Map<GetReviewsQuery>(request) with {HotelId = hotelId};
        var result = await _sender.Send(query);
        return result.Match(Ok,this.HandleFailure);
    }
    
    [HttpPost("/api/hotels/{hotelId}/reviews")]
    [Authorize]
    public async Task<IActionResult> PostReview(PostReviewRequest request,Guid hotelId)
    {
        var command = _mapper.Map<PostReviewCommand>(request) with {HotelId = hotelId, UserId = this.GetUserId()};
        var result = await _sender.Send(command);
        return result.Match(data=>
                Created($"/api/hotels/{hotelId}/reviews/",data),
            this.HandleFailure);
    }
    
    [HttpPut("/api/reviews/{reviewId}")]
    [Authorize]
    public async Task<IActionResult> PostReview(UpdateReviewRequest request,Guid reviewId)
    {
        var command = _mapper.Map<UpdateReviewCommand>(request) with
        {
            UserId = this.GetUserId(),
            ReviewId = reviewId
        };
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    [HttpDelete("/api/reviews/{reviewId}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(Guid reviewId)
    {
        var command = new DeleteReviewCommand(this.GetUserId(),reviewId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
}

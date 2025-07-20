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

    /// <summary>
    /// Retrieves reviews for a specific hotel.
    /// </summary>
    /// <param name="hotelId">The ID of the hotel to retrieve reviews for.</param>
    /// <param name="request">The request object containing pagination and filtering options.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns a list of reviews.</returns>
    /// <response code="200">Returns the list of reviews.</response>
    /// <response code="404">Hotel not found.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("/api/hotels/{hotelId}/reviews")]
    public async Task<IActionResult> GetReviews(Guid hotelId,[FromQuery] GetReviewsRequest request, CancellationToken cancellationToken)
    {
        var query = _mapper.Map<GetReviewsQuery>(request) with {HotelId = hotelId};
        var result = await _sender.Send(query,cancellationToken);
        return result.Match(Ok,this.HandleFailure);
    }
    
    /// <summary>
    /// Submits a new review for a hotel by the authenticated user.
    /// </summary>
    /// <param name="request">The review submission data.</param>
    /// <param name="hotelId">The ID of the hotel being reviewed.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns the created review.</returns>
    /// <response code="201">Review successfully created.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Hotel not found.</response>
    /// <response code="409">This Review already Exists.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("/api/hotels/{hotelId}/reviews")]
    [Authorize]
    public async Task<IActionResult> PostReview(PostReviewRequest request,Guid hotelId, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<PostReviewCommand>(request) with {HotelId = hotelId, UserId = this.GetUserId()};
        var result = await _sender.Send(command,cancellationToken);
        return result.Match(data=>
                Created($"/api/hotels/{hotelId}/reviews/",data),
            this.HandleFailure);
    }
    
    /// <summary>
    /// Updates a user's review.
    /// </summary>
    /// <param name="request">The updated review content.</param>
    /// <param name="reviewId">The ID of the review to update.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Review successfully updated.</response>
    /// <response code="400">Invalid data provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not the author of the review.</response>
    /// <response code="404">Review not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("/api/reviews/{reviewId}")]
    [Authorize]
    public async Task<IActionResult> PostReview(UpdateReviewRequest request,Guid reviewId, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateReviewCommand>(request) with
        {
            UserId = this.GetUserId(),
            ReviewId = reviewId
        };
        var result = await _sender.Send(command,cancellationToken);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    /// <summary>
    /// Deletes a review made by the authenticated user.
    /// </summary>
    /// <param name="reviewId">The ID of the review to delete.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Review successfully deleted.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not the author of the review.</response>
    /// <response code="404">Review not found.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("/api/reviews/{reviewId}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(Guid reviewId, CancellationToken cancellationToken)
    {
        var command = new DeleteReviewCommand(this.GetUserId(),reviewId);
        var result = await _sender.Send(command,cancellationToken);
        return result.Match(NoContent,this.HandleFailure);
    }
}

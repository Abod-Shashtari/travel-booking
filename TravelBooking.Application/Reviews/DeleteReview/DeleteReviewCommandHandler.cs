using AutoMapper;
using MediatR;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Reviews.Errors;

namespace TravelBooking.Application.Reviews.DeleteReview;

public class DeleteReviewCommandHandler:IRequestHandler<DeleteReviewCommand,Result>
{
    private readonly IRepository<Review> _reviewRepository;

    public DeleteReviewCommandHandler(IRepository<Review> reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review == null)
            return Result.Failure(ReviewErrors.ReviewNotFound());
        
        if(review.UserId != request.UserId)
            return Result.Failure(ReviewErrors.NotAllowedToUpdateThisReview());
        
        _reviewRepository.Delete(review);
        await _reviewRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
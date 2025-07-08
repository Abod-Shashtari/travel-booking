using MediatR;
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Application.Reviews.Specifications;
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
        var specification = new IncludeHotelWithReviewSpecification();
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId,specification, cancellationToken);
        if (review == null)
            return Result.Failure(ReviewErrors.ReviewNotFound());
        
        if(review.UserId != request.UserId)
            return Result.Failure(ReviewErrors.NotAllowedToUpdateThisReview());
        
        var specificationReview = new PaginationSpecification<Review>(1,0);
        var reviews= await _reviewRepository.GetPaginatedListAsync(specificationReview,cancellationToken);
        var reviewsCount = reviews.TotalCount;
        
        var hotel=review.Hotel!;
        if (reviewsCount == 1)
            hotel.StarRating = null;
        else
            hotel.StarRating = (hotel.StarRating*reviewsCount-review.StarRating)/(reviewsCount-1);
        
        _reviewRepository.Delete(review);
        await _reviewRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
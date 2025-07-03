using AutoMapper;
using MediatR;
using TravelBooking.Application.Reviews.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Reviews.Errors;

namespace TravelBooking.Application.Reviews.UpdateReview;

public class UpdateReviewCommandHandler:IRequestHandler<UpdateReviewCommand, Result>
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly IMapper _mapper;

    public UpdateReviewCommandHandler(IRepository<Review> reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var specification = new IncludeHotelWithReviewSpecification();
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, specification, cancellationToken);
        if (review == null)
            return Result.Failure(ReviewErrors.ReviewNotFound());
        
        if(review.UserId != request.UserId)
            return Result.Failure(ReviewErrors.NotAllowedToUpdateThisReview());
        
        _mapper.Map(request, review);
        review.Hotel!.StarRating = (review.Hotel!.StarRating+review.StarRating)/2;
        
        await _reviewRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
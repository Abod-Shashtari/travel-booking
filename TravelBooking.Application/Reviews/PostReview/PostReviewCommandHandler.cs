using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Reviews.Errors;

namespace TravelBooking.Application.Reviews.PostReview;

public class PostReviewCommandHandler:IRequestHandler<PostReviewCommand,Result<ReviewResponse?>>
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IMapper _mapper;

    public PostReviewCommandHandler(IRepository<Review> reviewRepository, IMapper mapper, IRepository<Hotel> hotelRepository)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
        _hotelRepository = hotelRepository;
    }

    public async Task<Result<ReviewResponse?>> Handle(PostReviewCommand request, CancellationToken cancellationToken)
    {
        var hotel=await _hotelRepository.GetByIdAsync(request.HotelId, cancellationToken);
        if(hotel == null)
            return Result<ReviewResponse?>.Failure(HotelErrors.HotelNotFound());
            
        var review = _mapper.Map<Review>(request);
        if (await _reviewRepository.IsExistAsync(review, cancellationToken))
            return Result<ReviewResponse?>.Failure(ReviewErrors.ReviewAlreadyExists());

        var specification = new PaginationSpecification<Review>(1,0);
        var reviews= await _reviewRepository.GetPaginatedListAsync(specification,cancellationToken);
        var reviewsCount = reviews.TotalCount;
        
        await _reviewRepository.AddAsync(review, cancellationToken);
        var newAvgStarRating = ((hotel.StarRating??0) * reviewsCount + review.StarRating) / (reviewsCount + 1);
        hotel.StarRating = hotel.StarRating==null?review.StarRating:newAvgStarRating;
        
        await _reviewRepository.SaveChangesAsync(cancellationToken);
        var reviewResponse=_mapper.Map<ReviewResponse>(review);
        return Result<ReviewResponse?>.Success(reviewResponse);
    }
}
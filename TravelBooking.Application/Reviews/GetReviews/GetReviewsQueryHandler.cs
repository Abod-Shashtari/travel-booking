using AutoMapper;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Application.Reviews.Specifications;
using TravelBooking.Domain.Common;
using TravelBooking.Domain.Common.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Errors;
using TravelBooking.Domain.Reviews.Entities;

namespace TravelBooking.Application.Reviews.GetReviews;

public class GetReviewsQueryHandler:IRequestHandler<GetReviewsQuery, Result<PaginatedList<ReviewResponse>?>>
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly IRepository<Hotel> _hotelRepository;
    private readonly IMapper _mapper;

    public GetReviewsQueryHandler(IRepository<Review> reviewRepository, IRepository<Hotel> hotelRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _hotelRepository = hotelRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<ReviewResponse>?>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        if (!await _hotelRepository.IsExistsByIdAsync(request.HotelId, cancellationToken))
            return Result<PaginatedList<ReviewResponse>?>.Failure(HotelErrors.HotelNotFound());

        var specification = new GetReviewsOfHotelSpecification(request.HotelId, request.PageNumber, request.PageSize);
        var reviews = await _reviewRepository.GetPaginatedListAsync(specification, cancellationToken);
        
        var mappedReviews = _mapper.Map<List<ReviewResponse>>(reviews.Data);
        var reviewsResponse= reviews.Map(mappedReviews);
        
        return Result<PaginatedList<ReviewResponse>?>.Success(reviewsResponse);
    }
}
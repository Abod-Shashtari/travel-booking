using AutoMapper;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Reviews.PostReview;
using TravelBooking.Application.Reviews.UpdateReview;
using TravelBooking.Domain.Reviews.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class ReviewProfile:Profile
{
    public ReviewProfile()
    {
        CreateMap<PostReviewCommand,Review>();
        CreateMap<UpdateReviewCommand,Review>();
        CreateMap<Review, ReviewResponse>();
    }
}
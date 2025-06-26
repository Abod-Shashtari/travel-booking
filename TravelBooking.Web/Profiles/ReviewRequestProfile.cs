using AutoMapper;
using TravelBooking.Application.Reviews.GetReviews;
using TravelBooking.Application.Reviews.PostReview;
using TravelBooking.Application.Reviews.UpdateReview;
using TravelBooking.Web.Requests.Reviews;

namespace TravelBooking.Web.Profiles;

public class ReviewRequestProfile:Profile
{
    public ReviewRequestProfile()
    {
        CreateMap<PostReviewRequest, PostReviewCommand>()
            .ForCtorParam(
                "HotelId",
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            )
            .ForCtorParam(
                "UserId",
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            );
        CreateMap<GetReviewsRequest, GetReviewsQuery>()
            .ForCtorParam(
                "HotelId",
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            );
        CreateMap<UpdateReviewRequest, UpdateReviewCommand>()
            .ForCtorParam(
                "UserId",
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            )
            .ForCtorParam(
                "ReviewId",
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            );
    }
}
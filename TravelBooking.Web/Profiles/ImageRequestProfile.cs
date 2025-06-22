using AutoMapper;
using TravelBooking.Application.Images.GetImages;
using TravelBooking.Web.Requests.Images;

namespace TravelBooking.Web.Profiles;

public class ImageRequestProfile:Profile
{
    public ImageRequestProfile()
    {
        CreateMap<GetImagesRequest, GetImagesQuery>()
            .ForCtorParam(
                "EntityId",
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            )
            .ForCtorParam(
                "EntityName",
                opt => opt.MapFrom((src, ctx) => string.Empty)
            );
    }
}
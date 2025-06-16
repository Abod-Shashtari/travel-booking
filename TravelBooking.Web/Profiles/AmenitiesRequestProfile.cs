using AutoMapper;
using TravelBooking.Application.Amenities.CreateAmenity;
using TravelBooking.Application.Amenities.GetAmenities;
using TravelBooking.Application.Amenities.UpdateAmenity;
using TravelBooking.Web.Requests.Amenities;

namespace TravelBooking.Web.Profiles;

public class AmenitiesRequestProfile:Profile
{
    public AmenitiesRequestProfile()
    {
        CreateMap<GetAmenitiesRequest, GetAmenitiesQuery>();
        CreateMap<CreateAmenityRequest, CreateAmenityCommand>();
        CreateMap<UpdateAmenityRequest, UpdateAmenityCommand>()
            .ForCtorParam(
                "AmenityId", 
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            );
    }
}
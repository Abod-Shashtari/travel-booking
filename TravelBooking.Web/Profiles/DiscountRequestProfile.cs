using AutoMapper;
using TravelBooking.Application.Discounts.CreateDiscount;
using TravelBooking.Application.Discounts.GetDiscounts;
using TravelBooking.Application.Discounts.UpdateDiscount;
using TravelBooking.Web.Requests.Discounts;

namespace TravelBooking.Web.Profiles;

public class DiscountRequestProfile:Profile
{
    public DiscountRequestProfile()
    {
        CreateMap<GetDiscountsRequest, GetDiscountsQuery>();
        CreateMap<CreateDiscountRequest, CreateDiscountCommand>();
        CreateMap<UpdateDiscountRequest, UpdateDiscountCommand>()
            .ForCtorParam(
                "DiscountId", 
                opt => opt.MapFrom((src, ctx) => Guid.Empty)
            );
    }
}

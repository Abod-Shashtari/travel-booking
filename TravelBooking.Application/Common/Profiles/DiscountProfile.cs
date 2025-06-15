using AutoMapper;
using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Discounts.CreateDiscount;
using TravelBooking.Application.Discounts.UpdateDiscount;
using TravelBooking.Domain.Discounts.Entities;

namespace TravelBooking.Application.Common.Profiles;

public class DiscountProfile:Profile
{
    public DiscountProfile()
    {
        CreateMap<CreateDiscountCommand, Discount>();
        CreateMap<UpdateDiscountCommand, Discount>();
        CreateMap<Discount,DiscountResponse>();
    }
}
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Discounts.GetDiscount;

public record GetDiscountQuery(Guid DiscountId):IRequest<Result<DiscountResponse?>>;
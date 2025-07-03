using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Discounts.DeleteDiscount;

public record DeleteDiscountCommand(Guid DiscountId):IRequest<Result>;
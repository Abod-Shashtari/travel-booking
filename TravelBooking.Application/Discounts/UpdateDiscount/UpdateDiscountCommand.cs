using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Discounts.UpdateDiscount;

public record UpdateDiscountCommand(Guid DiscountId, Guid RoomTypeId, decimal Percentage, DateTime StartDate, DateTime EndDate):IRequest<Result>;
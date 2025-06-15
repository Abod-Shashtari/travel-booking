using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Discounts.CreateDiscount;

public record CreateDiscountCommand(Guid RoomTypeId, decimal Percentage, DateTime StartDate, DateTime EndDate):IRequest<Result<DiscountResponse?>>;
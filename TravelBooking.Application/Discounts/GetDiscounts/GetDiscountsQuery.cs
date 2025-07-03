using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Discounts.GetDiscounts;

public record GetDiscountsQuery(int PageNumber = 1, int PageSize = 10):IRequest<Result<PaginatedList<DiscountResponse>>>;
using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Hotels.GetHotels;

public record GetHotelsQuery(HotelFilter? HotelFilter,int PageNumber, int PageSize):IRequest<Result<PaginatedList<HotelResponse>>>;
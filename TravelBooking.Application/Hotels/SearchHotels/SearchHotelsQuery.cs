using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Hotels.SearchHotels;

public record SearchHotelsQuery(HotelFilter? HotelFilter,int PageNumber, int PageSize):IRequest<Result<PaginatedList<HotelResponse>>>;
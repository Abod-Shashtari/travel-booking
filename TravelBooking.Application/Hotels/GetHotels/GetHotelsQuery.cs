﻿using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Hotels.GetHotels;

public record GetHotelsQuery(int PageNumber, int PageSize):IRequest<Result<PaginatedList<HotelResponse>>>;
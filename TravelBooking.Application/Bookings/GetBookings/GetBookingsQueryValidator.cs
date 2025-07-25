﻿using FluentValidation;
using TravelBooking.Application.Hotels.SearchHotels;

namespace TravelBooking.Application.Bookings.GetBookings;

public class GetBookingsQueryValidator:AbstractValidator<SearchHotelsQuery>
{
    public GetBookingsQueryValidator()
    {
        RuleFor(x=>x.PageNumber).GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");
    }
}
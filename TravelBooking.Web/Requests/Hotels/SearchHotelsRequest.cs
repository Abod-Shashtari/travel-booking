﻿namespace TravelBooking.Web.Requests.Hotels;

public record SearchHotelsRequest
{
    public string? Keyword {get; init;}
    public int? NumberOfRooms { get; init; } = 1;
    public int? NumberOfAdults { get; init; } = 2;
    public int? NumberOfChildren { get; init; } = 0;
    public DateTime? CheckIn {get; init;} = DateTime.Today;
    public DateTime? CheckOut {get; init;} = DateTime.Today.AddDays(1);
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public double? StarRating { get; init; }
    public List<string>? RoomTypes { get; set; }
    public List<string>? Amenities { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
};
using System.ComponentModel.DataAnnotations;

namespace TravelBooking.Application.Hotels.SearchHotel;

public record SearchHotelRequest
{
    public string? Keyword {get; init;}
    public int? NumberOfRooms { get; init; } = 1;
    public int? NumberOfAdults { get; init; } = 2;
    public int? NumberOfChildren { get; init; } = 0;
    public DateTime? CheckIn {get; init;} = DateTime.Today;
    public DateTime? CheckOut {get; init;} = DateTime.Today.AddDays(1);

    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be greater than 0.")]
    public int PageNumber { get; init; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; init; } = 10;
};
namespace TravelBooking.Application.Common.Models;

public record ReviewResponse(Guid Id, Guid UserId, Guid HotelId, string? TextReview, double StarRating);
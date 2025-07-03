namespace TravelBooking.Application.Common.Models;

public record JwtTokenResult(string Token, string Jti, DateTime ExpirationAt);
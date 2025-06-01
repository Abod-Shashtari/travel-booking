namespace TravelBooking.Domain.Entities;

public class TokenWhiteList:EntityBase
{
    public Guid UserId { get; set; }
    public string Token { get; set; }=string.Empty;
    public DateTimeOffset IssuedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
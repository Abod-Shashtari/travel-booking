using System.ComponentModel.DataAnnotations;

namespace TravelBooking.Domain.Entities;

public class TokenWhiteList:EntityBase
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public string Token { get; set; }=string.Empty;
    [Required]
    public DateTimeOffset IssuedAt { get; set; }
    [Required]
    public DateTimeOffset ExpiresAt { get; set; }
}
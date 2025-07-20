using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Domain.Tokens.Entities;

public class TokenWhiteList:EntityBase
{
    [Required]
    public Guid UserId { get; init; }
    public User? User { get; set; }
    [Required]
    public string Jti { get; init; } = string.Empty;
    [Required]
    public DateTimeOffset ExpiresAt { get; set; }
    [Required]
    public bool IsActive { get; set; } = true;
}
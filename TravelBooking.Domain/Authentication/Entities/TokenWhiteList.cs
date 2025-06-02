using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Common.Entities;

namespace TravelBooking.Domain.Authentication.Entities;

public class TokenWhiteList:EntityBase
{
    [Required]
    public Guid UserId { get; init; }
    [Required]
    public string Jti { get; init; } = string.Empty;
    [Required]
    public DateTimeOffset ExpiresAt { get; set; }
    [Required]
    public bool IsActive { get; set; } = true;
}
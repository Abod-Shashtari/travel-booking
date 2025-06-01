using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Enums;

namespace TravelBooking.Domain.Entities;

public class User:AuditEntity
{
    [MaxLength(50)]
    public string FirstName { get; set; }=string.Empty;
    [MaxLength(50)]
    public string LastName { get; set; }=string.Empty;
    [MaxLength(100)]
    public string Email { get; set; }=string.Empty;
    [MaxLength(128)]
    public string HashedPassword { get; set; } = string.Empty;
    public Roles Role { get; set; } = Roles.User;
}
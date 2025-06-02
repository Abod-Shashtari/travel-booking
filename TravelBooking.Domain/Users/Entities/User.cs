using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Common.Entities;

namespace TravelBooking.Domain.Users.Entities;

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
    public UserRole UserRole { get; set; } = UserRole.User;
}
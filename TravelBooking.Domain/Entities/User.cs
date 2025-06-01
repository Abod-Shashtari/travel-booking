using TravelBooking.Domain.Enums;

namespace TravelBooking.Domain.Entities;

public class User:AuditEntity
{
    public string Email { get; set; }=string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public Roles Role { get; set; } = Roles.User;
}
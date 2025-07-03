using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.UserActivity.Entites;

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
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<HotelVisit> HotelVisits { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
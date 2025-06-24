using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Domain.UserActivity.Entites;

public class HotelVisit:AuditEntity
{
    [Required]
    public Guid UserId { get; set; }
    public User? User { get; set; }
    [Required]
    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }
}
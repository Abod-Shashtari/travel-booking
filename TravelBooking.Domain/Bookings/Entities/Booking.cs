using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Domain.Bookings.Entities;

public class Booking:AuditEntity
{
    [Required]
    public Guid UserId { get; set; }
    public User? User { get; set; }
    [Required]
    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }
    
    [Required]
    public decimal TotalCost { get; set; }
    public ICollection<Room> Rooms { get; set; } = [];
    [Required]
    public DateTimeOffset CheckIn { get; set; }
    [Required]
    public DateTimeOffset CheckOut { get; set; }
    [Required]
    public BookingStatus Status { get; set; }
    public string? ConfirmationNumber { get; set; }
}
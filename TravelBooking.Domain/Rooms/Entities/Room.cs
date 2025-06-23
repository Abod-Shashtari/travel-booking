using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Domain.Rooms.Entities;

public class Room:AuditEntity
{
    [Required,MaxLength(20)]
    public string Number { get; set; } = string.Empty;
    [Required]
    public Guid RoomTypeId { get; set; }
    public RoomType? RoomType { get; set; }
    [Required]
    public int AdultCapacity { get; set; }
    [Required]
    public int ChildrenCapacity { get; set; }
    public ICollection<Booking> Bookings { get; set; } = [];
}
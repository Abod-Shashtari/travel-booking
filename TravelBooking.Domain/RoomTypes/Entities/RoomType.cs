using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Domain.RoomTypes.Entities;

public class RoomType:AuditEntity
{
    [Required]
    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }
    [Required,MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    [Required]
    public decimal PricePerNight { get; set; }
    public ICollection<Discount> Discounts { get; set; } = [];
    public ICollection<Room> Rooms { get; set; } = [];
    public ICollection<Amenity> Amenities { get; set; } = [];
}
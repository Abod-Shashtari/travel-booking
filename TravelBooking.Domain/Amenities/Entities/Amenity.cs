using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Domain.Amenities.Entities;

public class Amenity:AuditEntity
{
    [Required,MaxLength(100)]
    public string Name { get; set; }=string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    public ICollection<RoomType> RoomsTypes { get; set; } = [];
}
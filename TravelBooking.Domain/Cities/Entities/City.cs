using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Domain.Cities.Entities;

public class City:AuditEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string Country { get; set; } = string.Empty;
    [MaxLength(20)]
    public string PostOffice { get; set; } = string.Empty;
    public Guid? ThumbnailImageId { get; set; }
    public ICollection<Hotel> Hotels { get; set; } = [];
}
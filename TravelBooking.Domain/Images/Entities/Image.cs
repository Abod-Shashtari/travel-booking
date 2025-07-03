using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Common.Entities;

namespace TravelBooking.Domain.Images.Entities;

public class Image:AuditEntity
{
    [Required]
    public string Url { get; set; } = string.Empty;
    [Required]
    public Guid EntityId { get; set; }
    [Required]
    public EntityType EntityType { get; set; }
}
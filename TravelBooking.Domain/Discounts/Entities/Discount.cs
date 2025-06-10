using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.RoomTypes.Entities;

namespace TravelBooking.Domain.Discounts.Entities;

public class Discount:AuditEntity
{
    [Required]
    public Guid RoomTypeId { get; set; }
    public RoomType? RoomType { get; set; }
    [Required]
    public decimal Percentage { get; set; }
    [Required]
    public DateTime StartDate{ get; set; }
    [Required]
    public DateTime EndDate{ get; set; }
}
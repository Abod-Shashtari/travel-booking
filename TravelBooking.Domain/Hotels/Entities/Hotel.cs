using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Domain.Hotels.Entities;

public class Hotel:AuditEntity
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Location Location { get; set; }
    [Required]
    public Guid CityId { get; set; }
    public City City { get; set; }
    [Required]
    public Guid OwnerId { get; set; }
    public User Owner { get; set; }
}
using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.RoomTypes.Entities;
using TravelBooking.Domain.UserActivity.Entites;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Domain.Hotels.Entities;

public class Hotel:AuditEntity
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    [Required]
    public Location Location { get; set; }
    [Required]
    public Guid CityId { get; set; }
    public City? City { get; set; }
    [Required]
    public Guid OwnerId { get; set; }
    public User? Owner { get; set; }
    public double? StarRating { get; set; }
    public Guid? ThumbnailImageId { get; set; }
    public Image? ThumbnailImage { get; set; }
    public ICollection<RoomType> RoomTypes { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<HotelVisit> HotelVisits { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
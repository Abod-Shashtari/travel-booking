using System.ComponentModel.DataAnnotations;
using TravelBooking.Domain.Common.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Domain.Reviews.Entities;

public class Review : AuditEntity
{
    [MaxLength(500)]
    public string? TextReview { get; set; }
    [Required,Range(0,5)]
    public double StarRating { get; set; }
    [Required]
    public Guid UserId { get; set; }
    public User? User { get; set; }
    [Required]
    public Guid HotelId { get; set; }
    public Hotel? Hotel { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace TravelBooking.Domain.Hotels.Entities;

public record Location([Required,Range(-90, 90)] double Latitude, [Required,Range(-180, 180)] double Longitude);
namespace TravelBooking.Domain.Entities;

public class AuditEntity:EntityBase
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }
}
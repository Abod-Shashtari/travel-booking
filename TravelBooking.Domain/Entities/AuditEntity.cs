namespace TravelBooking.Domain.Entities;

public class AuditEntity:EntityBase
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }
    public string CreatedBy { get; set; }=string.Empty;
    public string ModifiedBy { get; set; }=string.Empty;
}
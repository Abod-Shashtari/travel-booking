
namespace TravelBooking.Application.Common.Models;

public record RoomResponse(
    Guid Id,
    string Number,
    Guid RoomTypeId,
    int AdultCapacity,
    int ChildrenCapacity,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ModifiedAt
);
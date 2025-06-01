namespace TravelBooking.Domain.Shared;

public record PaginatedList<T>(
    List<T> Data,
    int TotalCount,
    int PageSize,
    int CurrentPage)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}
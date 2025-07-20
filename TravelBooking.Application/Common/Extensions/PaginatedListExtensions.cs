using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Common.Extensions;

public static class PaginatedListExtensions
{
    public static PaginatedList<TDestination> Map<TSource, TDestination>(
        this PaginatedList<TSource> source,List<TDestination> responseList)
    {
        return new PaginatedList<TDestination>(
            responseList,
            source.TotalCount,
            source.PageSize,
            source.CurrentPage
        );
    }
}

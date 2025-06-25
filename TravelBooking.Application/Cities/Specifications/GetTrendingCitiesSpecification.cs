using TravelBooking.Application.Common.Models;
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.Cities.Entities;

namespace TravelBooking.Application.Cities.Specifications;

public class GetTrendingCitiesSpecification:PaginationSpecification<City>
{

    public GetTrendingCitiesSpecification(int numberOfCities)
        : base(1, numberOfCities, c=>c.Hotels.SelectMany(h=>h.HotelVisits).Count(), true)
    {
        Criteria = c => c.Hotels.Any(h=>h.HotelVisits.Count != 0);
    }
}
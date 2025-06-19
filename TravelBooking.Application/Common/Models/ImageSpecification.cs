using System.Linq.Expressions;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Application.Common.Models;

public class ImageSpecification:Specification<Image>
{
    public ImageSpecification(Expression<Func<Image, bool>> criteria)
    {
        Criteria=criteria;
    }
}
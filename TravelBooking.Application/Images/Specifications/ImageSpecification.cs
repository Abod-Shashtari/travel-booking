using System.Linq.Expressions;
using TravelBooking.Application.Common.Specifications;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Application.Images.Specifications;

public class ImageSpecification:PaginationSpecification<Image>
{
    public ImageSpecification(Expression<Func<Image, bool>> criteria,int pageSize,int pageNumber)
        : base(pageNumber, pageSize)
    {
        Criteria=criteria;
    }
}
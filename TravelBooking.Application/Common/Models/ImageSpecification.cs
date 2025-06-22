using System.Linq.Expressions;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Application.Common.Models;

public class ImageSpecification:PaginationSpecification<Image>
{
    public ImageSpecification(Expression<Func<Image, bool>> criteria,int pageSize,int pageNumber)
        : base(pageNumber, pageSize)
    {
        Criteria=criteria;
    }
}
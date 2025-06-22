using MediatR;
using TravelBooking.Application.Common.Models;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Images.GetImages;

public record GetImagesQuery(string EntityName, Guid EntityId,int PageNumber = 1, int PageSize = 10):IRequest<Result<PaginatedList<ImageResponse>?>>;

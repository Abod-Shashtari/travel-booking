using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Images.DeleteImage;
using TravelBooking.Application.Images.GetImages;
using TravelBooking.Application.Images.UploadImage;
using TravelBooking.Web.Extensions;
using TravelBooking.Web.Requests.Images;

namespace TravelBooking.Web.Controllers;

[ApiController]
public class ImageController:ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public ImageController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost("/api/{entityName}/{entityId}/images")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadImage(string entityName, Guid entityId, IFormFile image)
    {
        var command = new UploadImageCommand(entityName, entityId, image);
        var result = await _sender.Send(command);
        return result.Match(
            data=>
                Created($"/api/{data!.EntityType.ToString().ToLower()}/{data.EntityId}/images",data),
            this.HandleFailure
        );
    }
    
    [HttpDelete("/api/images/{imageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(Guid imageId)
    {
        var command = new DeleteImageCommand(imageId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    [HttpGet("/api/{entityName}/{entityId}/images")]
    public async Task<IActionResult> GetImages(string entityName, Guid entityId,[FromQuery]GetImagesRequest request)
    {
        var query = _mapper.Map<GetImagesQuery>(request) with {EntityName = entityName, EntityId = entityId };
        var result = await _sender.Send(query);
        return result.Match(Ok,this.HandleFailure);
    }
}
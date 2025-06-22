using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Images.DeleteImage;
using TravelBooking.Application.Images.UploadImage;
using TravelBooking.Web.Extensions;

namespace TravelBooking.Web.Controllers;

[ApiController]
public class ImageController:ControllerBase
{
    private readonly ISender _sender;
    
    public ImageController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("/api/{entityName}/{entityId}/images")]
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
    public async Task<IActionResult> DeleteImage(Guid imageId)
    {
        var command = new DeleteImageCommand(imageId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
}
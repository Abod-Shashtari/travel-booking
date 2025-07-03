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

    /// <summary>
    /// Uploads an image for a specific entity (Admin only).
    /// </summary>
    /// <param name="entityName">The name/type of the entity (e.g., "hotels", "room-types")</param>
    /// <param name="entityId">The ID of the target entity</param>
    /// <param name="image">The image file to upload</param>
    /// <returns>Created Image</returns>
    /// <response code="201">Image uploaded successfully</response>
    /// <response code="400">Invalid input or unsupported entity type</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="409">This Image already exists</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
    
    /// <summary>
    /// Deletes an image by ID (Admin only).
    /// </summary>
    /// <param name="imageId">The ID of the image to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">Image deleted successfully</response>
    /// <response code="404">Image not found</response>
    /// <response code="401">Unauthorized access</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete("/api/images/{imageId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(Guid imageId)
    {
        var command = new DeleteImageCommand(imageId);
        var result = await _sender.Send(command);
        return result.Match(NoContent,this.HandleFailure);
    }
    
    /// <summary>
    /// Retrieves images associated with a specific entity.
    /// </summary>
    /// <param name="entityName">The name/type of the entity</param>
    /// <param name="entityId">The ID of the target entity</param>
    /// <param name="request">pagination options</param>
    /// <returns>List of images</returns>
    /// <response code="200">Images retrieved successfully</response>
    /// <response code="400">Invalid entity or parameters</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("/api/{entityName}/{entityId}/images")]
    public async Task<IActionResult> GetImages(string entityName, Guid entityId,[FromQuery]GetImagesRequest request)
    {
        var query = _mapper.Map<GetImagesQuery>(request) with {EntityName = entityName, EntityId = entityId };
        var result = await _sender.Send(query);
        return result.Match(Ok,this.HandleFailure);
    }
}
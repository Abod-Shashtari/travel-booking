using TravelBooking.Domain.Common;

namespace TravelBooking.Domain.Images.Errors;

public class ImageErrors
{
    public static Error ImageAlreadyExists() =>
        new("Image.ImageAlreadyExists",
            "This Image is already exists in the system",
            ErrorType.Conflict
        );
    
    public static Error ImageNotFound() =>
        new("Image.NotFound",
            "Image with the specified ID was not found",
            ErrorType.NotFound
        );
    
    public static Error ImageInvalidEntityType() =>
        new("Image.InvalidEntityType",
            "This Entity Type is not valid for Image Uploading",
            ErrorType.BadRequest
        );
    
    public static Error ErrorWhileDeleting() =>
        new("Image.ErrorWhileDeleting",
            "An Error occurred while trying to delete the image",
            ErrorType.BadRequest
        );
}
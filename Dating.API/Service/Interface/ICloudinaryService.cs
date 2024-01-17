using CloudinaryDotNet.Actions;

namespace Dating.API.Service.Interface
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadPhoto(IFormFile file, object id);
    }
}
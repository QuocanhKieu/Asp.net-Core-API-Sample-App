using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using T2305M_API.DTO;
using T2305M_API.Entities;
using T2305M_API.Models;
using T2305M_API.Repositories;

namespace T2305M_API.Services.Implements
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _historyRepository;
        private readonly IWebHostEnvironment _env;

        public ImageService(IImageRepository historyRepository, IWebHostEnvironment env)
        {
            _historyRepository = historyRepository;
            _env = env;
        }
        public async Task<Dictionary<string, List<string>>> ValidateImageFiles(List<IFormFile> images)
        {
            var errors = new Dictionary<string, List<string>>();

            // Validate each image in the list
            foreach (var image in images)
            {
                var fileErrors = new List<string>();

                // Validate file size (limit to 5 MB)
                const long maxFileSize = 5 * 1024 * 1024;  // 5 MB
                if (image.Length > maxFileSize)
                {
                    fileErrors.Add("File size cannot exceed 5 MB.");
                }

                // Validate file type by checking extension
                var permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                {
                    fileErrors.Add("Invalid file extension. Only .jpg, .jpeg, and .png are allowed.");
                }

                // Validate MIME type to ensure it's an image
                if (!image.ContentType.StartsWith("image/"))
                {
                    fileErrors.Add("Only image files are allowed.");
                }

                // If any errors were found for this file, add them to the errors dictionary
                if (fileErrors.Count > 0)
                {
                    errors[image.FileName] = fileErrors;
                }
            }

            // Return the error dictionary if there are any errors, or return null if no errors
            return errors.Count > 0 ? errors : null;
        }


    }
}

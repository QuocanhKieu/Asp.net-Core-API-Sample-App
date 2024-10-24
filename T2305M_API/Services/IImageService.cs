using T2305M_API.DTO.History;
using T2305M_API.Models;

namespace T2305M_API.Services
{
    public interface IImageService
    {
        Task<Dictionary<string, List<string>>> ValidateImageFiles(List<IFormFile> images);
    }
}

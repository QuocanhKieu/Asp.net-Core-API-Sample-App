using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using T2305M_API.DTO.History;
using T2305M_API.DTO;
using T2305M_API.Models;
using T2305M_API.Repositories;

namespace T2305M_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImageController(IImageRepository iImageRepository)
        {
            _imageRepository = iImageRepository;
        }

        [HttpGet("get-images")]
        public async Task<IActionResult> GetImagesForEntity(int entityId, string entityType)
        {
            try
            {
                var images = await _imageRepository.GetImagesForEntity(entityId, entityType);
                return Ok(new APIResponse<List<GetBasicImageDTO>>(images, "Get images successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<PaginatedResult<GetBasicHistoryDTO>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }


    }

}

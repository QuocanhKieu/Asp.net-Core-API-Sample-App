
using Microsoft.AspNetCore.Mvc;


namespace T2305M_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("upload_image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "images", "userArticleContentImages");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }


            if (file == null || file.Length == 0)
            {
                return BadRequest("No image file received.");
            }

            // Validate the file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Invalid image file format. Only .jpg, .jpeg, .png are allowed.");
            }

            // Generate a unique filename
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(_env.WebRootPath, "uploads", "images", "userArticleContentImages", uniqueFileName);

            try
            {
                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Get the absolute URL of the uploaded image
                var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
                var imageUrl = $"{baseUrl}/uploads/images/userArticleContentImages/{uniqueFileName}";

                //return $"/uploads/images/userArticleThumbnails/{uniqueFileName}";


                return Ok(new { link = imageUrl  }); // This will be used by FroalaEditor to insert the image
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }


}

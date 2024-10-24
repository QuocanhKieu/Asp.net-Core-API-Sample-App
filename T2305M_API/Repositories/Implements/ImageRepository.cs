using Microsoft.AspNetCore.Mvc;
using System;
using T2305M_API.DTO;
using T2305M_API.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper; 

namespace T2305M_API.Repositories.Implements
{

    public class ImageRepository : IImageRepository
    {
        private readonly T2305mApiContext _context;
        private readonly IMapper _mapper;

        public ImageRepository(T2305mApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<GetBasicImageDTO>> GetImagesForEntity(int entityId, string entityType)
        {
            // Fetch all records if no parameters are provided
            if (entityId == null && entityType == null)
            {
                return await _context.Image
                    .Select(n => _mapper.Map<GetBasicImageDTO>(n))
                    .ToListAsync(); ;
            }

            // Fetch all records related to a specific entity type if only entityType is provided
            if (entityId == null && !string.IsNullOrEmpty(entityType))
            {
                return await _context.Image
                    .Where(img => img.RelatedEntityType == entityType)
                    .Select(n => _mapper.Map<GetBasicImageDTO>(n))
                    .ToListAsync();
            }

            // Fetch the specific record related to both entityId and entityType
            if (entityId != null && !string.IsNullOrEmpty(entityType))
            {
                return await _context.Image
                    .Where(img => img.RelatedEntityId == entityId && img.RelatedEntityType == entityType)
                    .Select(n => _mapper.Map<GetBasicImageDTO>(n))
                    .ToListAsync();
            }

            return new List<GetBasicImageDTO>();  // Return empty list as fallback, though it should never reach here
        }

        public async Task<CreateImagesResponseDTO> CreateImagesForEntity(CreateImagesDTO createImagesDTO)
        {
            var response = new CreateImagesResponseDTO();

            try
            {
                // Step 2: Save images to a physical location or cloud storage
                var savedImageUrls = new List<string>();
                foreach (var formFile in createImagesDTO.Images)
                {
                    if (formFile.Length > 0)
                    {
                        // Generate unique filename for each image
                        var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(formFile.FileName);

                        // Save to a local folder (for example: "wwwroot/uploads")
                        var filePath = Path.Combine("wwwroot/uploads/images_1", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        // Store the URL to the saved image
                        var imageUrl = $"/uploads/images_1/{fileName}";
                        savedImageUrls.Add(imageUrl);
                    }
                }

                // Step 3: Save image data to the database
                var newImages = new List<Image>();
                foreach (var imageUrl in savedImageUrls)
                {
                    var image = new Image
                    {
                        Url = imageUrl,
                        RelatedEntityId = createImagesDTO.RelatedEntityId,
                        RelatedEntityType = createImagesDTO.RelatedEntityType
                    };

                    newImages.Add(image);
                }

                // Save all image records to the database
                _context.Image.AddRange(newImages);
                await _context.SaveChangesAsync();

                // Step 4: Return success response
                response.isSuccess = true;
                response.Message = $"{newImages.Count} images successfully uploaded and saved.";
                return response;
            }
            catch (Exception ex)
            {
                // Handle error and return failure response
                throw;
            }
        }

    }

}

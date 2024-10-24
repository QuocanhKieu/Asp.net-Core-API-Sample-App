using T2305M_API.DTO.UserArticle;
using T2305M_API.Entities;
using T2305M_API.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using T2305M_API.Repositories;
using T2305M_API.DTO.Book;
using T2305M_API.Repositories.Implements;
using Microsoft.EntityFrameworkCore;


namespace T2305M_API.Services.Implements
{
    public class UserArticleService : IUserArticleService
    {
        private readonly T2305mApiContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IUserArticleRepository _userArticleRepository;

        public UserArticleService(T2305mApiContext context, IWebHostEnvironment env, IUserArticleRepository userArticleRepository)
        {
            _env = env;
            _userArticleRepository = userArticleRepository;
            _context = context;
        }
        public async Task<PaginatedResult<GetBasicUserArticleDTO>> GetBasicUserArticleDTOsAsync(UserArticleQueryParameters queryParameters)
        {
            // Fetch user articles from the repository based on the query parameters
            var (data, totalItems) = await _userArticleRepository.GetUserArticlesAsync(queryParameters);

            var basicUserArticleDTOs = new List<GetBasicUserArticleDTO>();

            foreach (var userArticle in data)
            {
                // Fetch the tags associated with the user article
                var UserArticleTagDTOs = userArticle.userArticleUserArticleTags?.Select(bt => new UserArticleTagDTO
                {
                    UserArticleTagId = bt.UserArticleTagId,
                    Name = bt.UserArticleTag?.Name ?? "",
                }).ToList() ?? new List<UserArticleTagDTO>();

                // Create a new GetBasicUserArticleDTO and populate its properties
                var basicUserArticleDTO = new GetBasicUserArticleDTO
                {
                    UserArticleId = userArticle.UserArticleId, // Assuming there's a UserArticleId property
                    Title = userArticle.Title, // Assuming there's a Title property
                    Description = userArticle.Description, // Assuming there's a Description property
                    ThumbnailImage = userArticle.ThumbnailImage, // Assuming there's a ThumbnailImage property
                    IsPromoted = userArticle.IsPromoted, // Assuming there's an IsPromoted property
                    UserId = userArticle.UserId, // Assuming there's a UserId property
                    UserName = userArticle.User?.FullName ?? "", // Assuming there's a UserName property
                    CreatedAt = userArticle.CreatedAt,
                    Status = userArticle.Status, // Assuming there's an IsActive property
                    UserArticleTags = UserArticleTagDTOs // Add the associated tags
                };

                // Add the populated DTO to the list
                basicUserArticleDTOs.Add(basicUserArticleDTO);
            }

            // Calculate total pages
            int totalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize);

            // Return the paginated result
            return new PaginatedResult<GetBasicUserArticleDTO>
            {
                TotalItems = totalItems,
                PageSize = queryParameters.PageSize,
                CurrentPage = queryParameters.Page,
                TotalPages = totalPages,
                HasNextPage = queryParameters.Page < totalPages,
                HasPreviousPage = queryParameters.Page > 1,
                Data = basicUserArticleDTOs
            };
        }

        public async Task<GetDetailUserArticleDTO> GetDetailUserArticleDTOByIdAsync(int userArticleId)
        {
            // Fetch the user article by ID from the repository
            var userArticle = await _userArticleRepository.GetUserArticleByIdAsync(userArticleId);

            if (userArticle == null)
            {
                return null; // Or throw an appropriate exception
            }

            // Map the userArticle to GetDetailUserArticleDTO
            var detailUserArticleDTO = new GetDetailUserArticleDTO
            {
                UserArticleId = userArticle.UserArticleId, // Assuming there is a UserArticleId property
                Title = userArticle.Title, // Assuming there is a Title property
                Description = userArticle.Description, // Assuming there is a Description property
                Content = userArticle.Content, // Assuming there is a Content property
                ThumbnailImage = userArticle.ThumbnailImage, // Assuming there is a ThumbnailImage property
                IsPromoted = userArticle.IsPromoted, // Assuming there is an IsPromoted property
                UserId = userArticle.UserId, // Assuming there is a UserId property
                UserName = userArticle.User?.FullName ?? "", // Assuming there's a UserName property
                CreatedAt = userArticle.CreatedAt,
                UpdatedAt = userArticle.UpdatedAt, // Assuming there is an UpdatedAt property
                Status = userArticle.Status // Assuming there is an IsActive property

            };

            return detailUserArticleDTO;
        }

        public async Task<CreateUserArticleResponseDTO> CreateUserArticleAsync(int userId, CreateUserArticleDTO createUserArticleDTO, IFormFile image)
        {
            // Ensure the wwwroot/images/userArticleThumbnails directory exists
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/images/userArticleThumbnails");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate a unique file name
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                // Save the image
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return new CreateUserArticleResponseDTO { Message = "Error saving the file: " + ex.Message };
            }

            // Store the relative image path
            //createUserArticleDTO.ThumbnailImage = $"/uploads/images/userArticleThumbnails/{uniqueFileName}";
            var newPath = $"/uploads/images/userArticleThumbnails/{uniqueFileName}";

            // Delegate data saving to the repository layer
            return await _userArticleRepository.CreateUserArticleAsync(userId, createUserArticleDTO, newPath);

        }

        public async Task<UpdateUserArticleResponseDTO> UpdateUserArticleAsync( UpdateUserArticleDTO updateUserArticleDTO, IFormFile image)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                string newImagePath = "";
                string oldImagePath = "";
                try
                {
                    UserArticle userArticle = await _userArticleRepository.UpdateUserArticleAsync(updateUserArticleDTO);
                    if (userArticle != null)
                    {
                        newImagePath = await SaveImageAsync(image);
                        oldImagePath = userArticle.ThumbnailImage;
                        userArticle.ThumbnailImage = newImagePath;
                        _context.UserArticle.Update(userArticle);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception();
                    }

                    // Commit the transaction first (if the database update succeeds)
                    await transaction.CommitAsync();

                    // Now that the database update succeeded, delete the old image if necessary
                    if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath != "/uploads/images/userArticleThumbnails/default-article-thumbnail.jpg")
                    {
                        DeleteOldImage(oldImagePath);
                    }

                    return new UpdateUserArticleResponseDTO
                    {
                        UserArticleId = userArticle.UserArticleId,
                        Message = "UserArticle updated successfully",
                        Status  = userArticle.Status,
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    // If a new image was saved before failure, optionally delete it to clean up
                    if (image != null)
                    {
                        if(!string.IsNullOrEmpty(newImagePath)) // save image not successfully then no deletion
                        DeleteOldImage(newImagePath);
                    }

                    return new UpdateUserArticleResponseDTO { Message = "Error updating UserArticle: " + ex.Message };
                }
            }
        }

        public async Task<Dictionary<string, List<string>>> ValidateCreateUserArticleDTO(CreateUserArticleDTO createUserArticleDTO)
        {
            var errors = new Dictionary<string, List<string>>();

            //Validate CustomerId
            if (string.IsNullOrEmpty(createUserArticleDTO.Title))
            {
                AddError(errors, "Title", "Title is required.");
            }

            return errors.Count > 0 ? errors : null;
        }
        public async Task<Dictionary<string, List<string>>> ValidateImageFile(IFormFile image)
        {
            var errors = new Dictionary<string, List<string>>();

            // Validate file size (example: limit to 5 MB)
            const long maxFileSize = 5 * 1024 * 1024;  // 5 MB
            if (image.Length > maxFileSize)
            {
                AddError(errors, "Thumbnail", "File size cannot exceed 5 MB.");
            }

            // Validate file type by checking MIME type and extension
            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
            {
                AddError(errors, "Thumbnail", "Invalid file extension. Only .jpg, .jpeg, .png are allowed.");
            }

            if (!image.ContentType.StartsWith("image/"))
            {
                AddError(errors, "Thumbnail", "Only image files are allowed.");
            }
            return errors.Count > 0 ? errors : null;
        }
        private static void AddError(Dictionary<string, List<string>> errors, string key, string errorMessage)
        {
            if (!errors.ContainsKey(key))
            {
                errors[key] = new List<string>();
            }
            errors[key].Add(errorMessage);
        }
        // Separate method for saving the image
        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/images/userArticleThumbnails");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"/uploads/images/userArticleThumbnails/{uniqueFileName}";
        }

        // Method to delete the old image
        private void DeleteOldImage(string oldImagePath)
        {
            var filePath = Path.Combine(_env.WebRootPath, oldImagePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}

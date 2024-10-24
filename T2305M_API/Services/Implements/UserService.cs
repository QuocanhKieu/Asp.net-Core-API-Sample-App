using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using T2305M_API.DTO.Event;
using T2305M_API.DTO.History;
using T2305M_API.DTO.User;
using T2305M_API.DTO.User;
using T2305M_API.DTO.UserArticle;
using T2305M_API.Entities;
using T2305M_API.Repositories;
using T2305M_API.Repositories.Implements;

namespace T2305M_API.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _env;
        private readonly T2305mApiContext _context;
        private readonly IMapper _mapper;


        public UserService(T2305mApiContext context, IUserRepository userRepository, IWebHostEnvironment env, ICreatorRepository creatorRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _env = env;
            _mapper = mapper;
            _context = context;
        }
        public async Task<GetDetailUserDTO> GetDetailUserDTOByIdAsync(int userId)
        {
            // Fetch the user entity by ID
/*            var userEntity = await _userRepository.GetUserByIdAsync(userId)*/;
            var userEntity = await _userRepository.GetUserByIdAsync(userId,  true, true);


            if (userEntity == null)
            {
                return null; // Or throw an appropriate exception if you prefer
            }

            //var variable =  userEntity.UserEvents.Count;
            // Map the user entity to the GetDetailUserDTO
            var detailUserDTO = new GetDetailUserDTO
            {
                UserId = userEntity.UserId,
                FullName = userEntity.FullName,
                Email = userEntity.Email,
                Age = userEntity.Age,
                Education = userEntity.Education,
                ShortBiography = userEntity.ShortBiography,
                LongBiography = userEntity.LongBiography,
                PhotoUrl = userEntity.PhotoUrl,
                Facebook = userEntity.Facebook,
                LinkedIn = userEntity.LinkedIn,
                Twitter = userEntity.Twitter,
                PersonalWebsiteUrl = userEntity.PersonalWebsiteUrl,
                PersonalWebsiteTitle = userEntity.PersonalWebsiteTitle,
                ReceiveNotifications = userEntity.ReceiveNotifications,
                IsActive = userEntity.IsActive,
                

                // Mapping UserEvents to BasicUserSavedEventDTO
                BasicUserSavedEvents = userEntity.UserEvents?
                    .Where(e => e.Event != null) // Ensure that the Event is not null
                    .Select(e =>  _mapper.Map<GetBasicEventDTO>(e.Event))
                    .ToList(),

                // Mapping UserArticles to BasicUserArticleDTO
                BasicUserArticles = userEntity.UserArticles?.Select(a => new GetBasicUserArticleDTO
                {
                    UserArticleId = a.UserArticleId,
                    Title = a.Title,
                    Description = a.Description,
                    ThumbnailImage = a.ThumbnailImage,
                    IsPromoted = a.IsPromoted,
                    UserId = a.UserId,
                    UserName = a.User != null ? a.User.FullName : "Unknown", // Assuming User has a Name property
                    CreatedAt = a.CreatedAt,
                    Status = a.Status,
                    UserArticleTags = a.userArticleUserArticleTags?.Select(tag => new UserArticleTagDTO
                    {
                        UserArticleTagId = tag.UserArticleTag.UserArticleTagId,
                        Name = tag.UserArticleTag.Name
                    }).ToList()
                }).ToList()
            };

            return detailUserDTO;
        }

        public async Task<UpdateUserResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO updateUserDTO)
        {
            UpdateUserResponseDTO updateUserResponseDTO = await _userRepository.UpdateUserAsync( userId,  updateUserDTO);
            return updateUserResponseDTO;
        }

        public async Task<Dictionary<string, List<string>>> ValidateUpdateUserDTO(UpdateUserDTO updateUserDTO)
        {
            var errors = new Dictionary<string, List<string>>();

            //Validate CustomerId
            //if (updateUserDTO.UserId <= 0 || updateUserDTO.UserId != UserId)
            //{
            //    AddError(errors, "UserId", "UserId is not provided or UserId mismatch.");
            //}

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


        public async Task<UpdateAvatarResponseDTO> UploadAvatarAsync(int userId, IFormFile file)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new FileNotFoundException("User not found.");
            }

                // Delete old avatar if it's not the default
                if (!string.IsNullOrEmpty(user.PhotoUrl) && user.PhotoUrl != "/uploads/avatars/default-avatar.png")
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, user.PhotoUrl.TrimStart('/'));
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }
            

            // Generate new file path
            var fileName = $"{user.UserId}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_env.WebRootPath, "uploads/avatars", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update user's AvatarUrl
            user.PhotoUrl = $"/uploads/avatars/{fileName}";
            await _userRepository.UpdateUserImageAsync(user);

            return new UpdateAvatarResponseDTO
            {
                UserId = userId,
                FilePath = user.PhotoUrl,
                Message  = "File Uploaded Successfully",
            };
        }
    }
}

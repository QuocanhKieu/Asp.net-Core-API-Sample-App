using T2305M_API.DTO.UserArticle;
using T2305M_API.Entities;
using T2305M_API.Models;

namespace T2305M_API.Services
{
    public interface IUserArticleService
    {
        Task<PaginatedResult<GetBasicUserArticleDTO>> GetBasicUserArticleDTOsAsync(UserArticleQueryParameters queryParameters);
        Task<GetDetailUserArticleDTO> GetDetailUserArticleDTOByIdAsync(int userArticleId);
        Task<Dictionary<string, List<string>>> ValidateCreateUserArticleDTO(CreateUserArticleDTO createUserArticleDTO);
        Task<Dictionary<string, List<string>>> ValidateImageFile(IFormFile image);
        Task<CreateUserArticleResponseDTO> CreateUserArticleAsync(int userId, CreateUserArticleDTO createUserArticleDTO, IFormFile image);
        Task<UpdateUserArticleResponseDTO> UpdateUserArticleAsync( UpdateUserArticleDTO updateUserArticleDTO, IFormFile image);
    }
}

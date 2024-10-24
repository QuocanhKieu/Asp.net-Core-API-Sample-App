using T2305M_API.DTO.UserArticle;
using T2305M_API.Entities;

namespace T2305M_API.Repositories
{
    public interface IUserArticleRepository
    {
        Task<(IEnumerable<UserArticle> Data, int TotalItems)> GetUserArticlesAsync(UserArticleQueryParameters queryParameters);
        Task<CreateUserArticleResponseDTO> CreateUserArticleAsync(int userId, CreateUserArticleDTO createUserArticleDTO, string newPath);
        Task<UserArticle> UpdateUserArticleAsync(UpdateUserArticleDTO updateUserArticleDTO);
        Task<UserArticle> GetUserArticleByIdAsync(int userArticleId);
        Task<ChangeUserArticleStatusResponseDTO> SetUserArticleInactive(UserArticle userArticle);
        Task<ChangeUserArticleStatusResponseDTO> SetUserArticleActive(UserArticle userArticle);
        Task<ChangeUserArticleStatusResponseDTO> SetUserArticleApproved(UserArticle userArticle);
    }
}

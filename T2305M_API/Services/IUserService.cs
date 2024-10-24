using T2305M_API.DTO.User;
using T2305M_API.DTO.User;

namespace T2305M_API.Services
{
    public interface IUserService
    {
        Task<GetDetailUserDTO> GetDetailUserDTOByIdAsync(int userId);
        Task<UpdateUserResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO user);
        Task<Dictionary<string, List<string>>> ValidateUpdateUserDTO( UpdateUserDTO updateUserDTO);
        Task<UpdateAvatarResponseDTO> UploadAvatarAsync(int userId, IFormFile file);

    }
}


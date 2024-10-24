using T2305M_API.DTO.Event;
using T2305M_API.DTO.UserEvent;
using T2305M_API.Entities;
using T2305M_API.Models;

namespace T2305M_API.Repositories
{
    public interface IUserEventRepository
    {
        //Task<UserEventResponseDTO> CreateBookmark(int userId, int eventId);
        //Task<UserEventResponseDTO> RemoveBookmark(int userId, int eventId);
        Task<PaginatedResult<GetUserEventDTO>> GetBasicUserEventDTOsAsync(int userId, UserEventQueryParameters queryParameters);
        Task<GetDetailUserEventDTO> GetDetailUserEventDTO(int userId, int orderId);


    }
}

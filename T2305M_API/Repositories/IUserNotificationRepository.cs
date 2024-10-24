using T2305M_API.DTO.Notification;
using T2305M_API.Entities.Notification;
using T2305M_API.Models;

namespace T2305M_API.Repositories
{
    public interface IUserNotificationRepository
    {
        Task<CreateBasicNotificationResponseDTO> CreateNotificationAsync(CreateBasicNotificationDTO createBasicNotificationDTO, int userId);
        Task<PaginatedResult<GetBasicNotificationDTO>> GetUserNotificationsAsync(NotificationQueryParameters queryParameters);
        Task MarkAsReadAsync(int notificationId);
    }

}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using T2305M_API.DTO.Notification;
using T2305M_API.Models;
using T2305M_API.Repositories; // Assuming you have a repository for notifications

namespace T2305M_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserNotificationController : ControllerBase
    {
        private readonly IUserNotificationRepository _notificationRepository;

        public UserNotificationController(IUserNotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        // GET: api/UserNotification
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<APIResponse<PaginatedResult<GetBasicNotificationDTO>>>> GetUserNotifications([FromQuery] NotificationQueryParameters queryParameters)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token or user not authenticated" });
                }


                int userId = int.Parse(userIdClaim);
                queryParameters.UserId = userId;
                                var paginatedResult = await _notificationRepository.GetUserNotificationsAsync(queryParameters);
                return Ok(new APIResponse<PaginatedResult<GetBasicNotificationDTO>>(paginatedResult, "Notifications retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<PaginatedResult<GetBasicNotificationDTO>>(HttpStatusCode.InternalServerError, "An error occurred while retrieving notifications.", ex.Message));
            }
        }

        // PUT: api/UserNotification/mark-as-read/{notificationId}
        [Authorize]
        [HttpPut("mark-as-read/{notificationId}")]
        public async Task<ActionResult<APIResponse<bool>>> MarkAsRead(int notificationId)
        {
            try
            {
                await _notificationRepository.MarkAsReadAsync(notificationId);
                return Ok(new APIResponse<bool>(true, "Notification marked as read successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new APIResponse<bool>(HttpStatusCode.InternalServerError, "An error occurred while marking the notification as read.", ex.Message));
            }
        }
    }
}

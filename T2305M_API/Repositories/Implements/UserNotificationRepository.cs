using AutoMapper; // Assuming you are using AutoMapper for DTO mapping
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using T2305M_API.DTO.Notification;
using T2305M_API.Entities;
using T2305M_API.Entities.Notification;
using T2305M_API.Models;

namespace T2305M_API.Repositories.Implements
{
    public class UserNotificationRepository : IUserNotificationRepository
    {
        private readonly T2305mApiContext _context;
        private readonly IMapper _mapper; // For mapping DTOs to entities and vice versa

        public UserNotificationRepository(T2305mApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CreateBasicNotificationResponseDTO> CreateNotificationAsync(CreateBasicNotificationDTO createBasicNotificationDTO, int userId)
        {
            UserNotification notification = _mapper.Map<UserNotification>(createBasicNotificationDTO); // Map DTO to entity
            notification.UserId = userId; // Set the UserId to associate the notification with the user

            await _context.UserNotification.AddAsync(notification);
            await _context.SaveChangesAsync();

            return new CreateBasicNotificationResponseDTO
            {
                UserNotificationId = notification.UserNotificationId,
                Message = "Create Notification successfully",
            }; // Return the created notification as a DTO
        }

        public async Task<PaginatedResult<GetBasicNotificationDTO>> GetUserNotificationsAsync(NotificationQueryParameters queryParameters)
        {
            var query = _context.UserNotification.Include(n => n.User).AsQueryable(); // Create a queryable for flexibility

            // Apply filters based on the query parameters
            if (queryParameters.UserId > 0)
            {
                query = query.Where(n => n.UserId == queryParameters.UserId);
            }

            if (queryParameters.IsRead.HasValue)
            {
                query = query.Where(n => n.IsRead == queryParameters.IsRead.Value);
            }

            // Sorting notifications
            bool isDescending = queryParameters.SortOrder?.ToLower() == "desc";
            switch (queryParameters.SortColumn?.ToLower())
            {
                case "message":
                    query = isDescending ? query.OrderByDescending(n => n.Message) : query.OrderBy(n => n.Message);
                    break;
                case "createdat":
                    query = isDescending ? query.OrderByDescending(n => n.CreatedAt) : query.OrderBy(n => n.CreatedAt);
                    break;
                default:
                    query = isDescending ? query.OrderByDescending(n => n.CreatedAt) : query.OrderBy(n => n.CreatedAt); // Default to sorting by CreatedAt
                    break;
            }

            // Pagination
            int totalItems = await query.CountAsync();
            var pagedData = await query
                .Skip((queryParameters.Page - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .Select(n => _mapper.Map<GetBasicNotificationDTO>(n)) // Map entities to DTOs
                .ToListAsync();

            return new PaginatedResult<GetBasicNotificationDTO>
            {
                TotalItems = totalItems,
                PageSize = queryParameters.PageSize,
                CurrentPage = queryParameters.Page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)queryParameters.PageSize),
                HasNextPage = queryParameters.Page < (int)Math.Ceiling(totalItems / (double)queryParameters.PageSize),
                HasPreviousPage = queryParameters.Page > 1,
                Data = pagedData
            };
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.UserNotification.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
            
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using T2305M_API.DTO.Event;
using T2305M_API.DTO.UserEvent;
using T2305M_API.Entities;
using T2305M_API.Models;

namespace T2305M_API.Repositories.Implements
{
    public class UserEventRepository : IUserEventRepository
    {
        private readonly T2305mApiContext _context;
        private readonly IWebHostEnvironment _env;

        public UserEventRepository(T2305mApiContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //public async Task<UserEventResponseDTO> CreateBookmark(int userId,int eventId)
        //{
        //    using (var transaction = await _context.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // Check if User exists
        //            var user = await _context.User.FindAsync(userId);
        //            if (user == null)
        //            {
        //                throw new KeyNotFoundException($"User with ID {userId} not found.");
        //            }

        //            // Check if Event exists
        //            var eventEntity = await _context.Event.FindAsync(eventId);
        //            if (eventEntity == null)
        //            {
        //                throw new KeyNotFoundException($"Event with ID {eventId} not found.");
        //            }

        //            // Check if the bookmark already exists (to prevent duplicates)
        //            var existingBookmark = await _context.UserEvent
        //                .FirstOrDefaultAsync(ue => ue.UserId == userId && ue.EventId == eventId);

        //            if (existingBookmark != null)
        //            {
        //                throw new InvalidOperationException("Bookmark already exists for this user and event.");
        //            }

        //            // Create new bookmark (UserEvent)
        //            var newUserEvent = new UserEvent
        //            {
        //                UserId = userId,
        //                EventId = eventId,
        //                CreatedAt = DateTime.Now
        //            };

        //            _context.UserEvent.Add(newUserEvent);
        //            await _context.SaveChangesAsync();

        //            // Commit the transaction
        //            await transaction.CommitAsync();

        //            // Return success response
        //            return new UserEventResponseDTO
        //            {
        //                EventId = eventId,
        //                Message = "Bookmark added successfully"
        //            };
        //        }
        //        catch (KeyNotFoundException ex) // Handle missing user/event errors
        //        {
        //            await transaction.RollbackAsync();
        //            throw new ArgumentException(ex.Message); // Throw specific exception for missing keys
        //        }
        //        catch (InvalidOperationException ex) // Handle already existing bookmark case
        //        {
        //            await transaction.RollbackAsync();
        //            throw new InvalidOperationException(ex.Message); // Throw for duplicate bookmark
        //        }
        //        catch (DbUpdateException dbEx) // Handle database-specific errors
        //        {
        //            await transaction.RollbackAsync();
        //            throw new DbUpdateException("Database update failed during bookmark creation: " + dbEx.Message, dbEx);
        //        }
        //        catch (Exception ex) // Handle all other errors
        //        {
        //            await transaction.RollbackAsync();
        //            throw new Exception("An error occurred while creating the bookmark: " + ex.Message, ex);
        //        }
        //    }
        //}

        //public async Task<UserEventResponseDTO> RemoveBookmark(int userId, int eventId)
        //{
        //    using (var transaction = await _context.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // Check if User exists
        //            var user = await _context.User.FindAsync(userId);
        //            if (user == null)
        //            {
        //                throw new KeyNotFoundException($"User with ID {userId} not found.");
        //            }

        //            // Check if Event exists
        //            var eventEntity = await _context.Event.FindAsync(eventId);
        //            if (eventEntity == null)
        //            {
        //                throw new KeyNotFoundException($"Event with ID {eventId} not found.");
        //            }

        //            // Check if the bookmark already exists
        //            var existingBookmark = await _context.UserEvent
        //                .FirstOrDefaultAsync(ue => ue.UserId == userId && ue.EventId == eventId);

        //            // If no bookmark exists, throw an exception
        //            if (existingBookmark == null)
        //            {
        //                throw new InvalidOperationException("Bookmark does not exist for this user and event.");
        //            }

        //            // Remove the bookmark
        //            _context.UserEvent.Remove(existingBookmark);
        //            await _context.SaveChangesAsync();

        //            // Commit the transaction
        //            await transaction.CommitAsync();

        //            return new UserEventResponseDTO
        //            {
        //                EventId = eventId,
        //                Message = "Bookmark removed successfully"
        //            };
        //        }
        //        catch (KeyNotFoundException ex)
        //        {
        //            await transaction.RollbackAsync();
        //            throw new ArgumentException(ex.Message);
        //        }
        //        catch (InvalidOperationException ex)
        //        {
        //            await transaction.RollbackAsync();
        //            throw new InvalidOperationException(ex.Message);
        //        }
        //        catch (DbUpdateException dbEx)
        //        {
        //            await transaction.RollbackAsync();
        //            throw new DbUpdateException("Database update failed: " + dbEx.Message, dbEx);
        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();
        //            throw new Exception("An error occurred: " + ex.Message, ex);
        //        }
        //    }
        //}

        public async Task<PaginatedResult<GetUserEventDTO>> GetBasicUserEventDTOsAsync(int userId, UserEventQueryParameters queryParameters)
        {
            // Step 1: Query the Orders where the userId matches
            IQueryable<Order> query = _context.Order.Where(o => o.UserId == userId)
                                                     .Include(o => o.Event); // Include the related Event

            // Step 2: Apply filters based on Event properties

            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                query = query.Where(o =>
                    o.Event.Title.Contains(queryParameters.SearchTerm) ||
                    o.Event.Description.Contains(queryParameters.SearchTerm) ||
                    o.Event.Organizer.Contains(queryParameters.SearchTerm) ||
                    o.Event.Location.Contains(queryParameters.SearchTerm) ||
                    o.Event.Address.Contains(queryParameters.SearchTerm));
            }

            // Step 3: Apply sorting
            if (!string.IsNullOrEmpty(queryParameters.SortColumn))
            {
                bool isDescending = queryParameters.SortOrder?.ToLower() == "desc";
                switch (queryParameters.SortColumn.ToLower())
                {
                    case "title":
                        query = isDescending ? query.OrderByDescending(o => o.Event.Title) : query.OrderBy(o => o.Event.Title);
                        break;
                    case "quantity":
                        query = isDescending ? query.OrderByDescending(o => o.Quantity) : query.OrderBy(o => o.Quantity);
                        break;
                    default:
                        query = isDescending ? query.OrderByDescending(o => o.Event.StartDate) : query.OrderBy(o => o.Event.StartDate);
                        break;
                }
            }

            // Step 4: Pagination
            int totalItems = await query.CountAsync();
            var pagedData = await query
                .Skip((queryParameters.Page - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToListAsync();

            // Step 5: Manually map the result to GetUserEventDTO
            var userEventDTOs = pagedData.Select(o => new GetUserEventDTO
            { 
                OrderId = o.OrderId,
                EventId = o.Event.EventId,
                Title = o.Event.Title,
                Quantity = o.Quantity
            }).ToList();

            // Step 6: Calculate total pages
            int totalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize);

            // Return the paginated result
            return new PaginatedResult<GetUserEventDTO>
            {
                TotalItems = totalItems,
                PageSize = queryParameters.PageSize,
                CurrentPage = queryParameters.Page,
                TotalPages = totalPages,
                HasNextPage = queryParameters.Page < totalPages,
                HasPreviousPage = queryParameters.Page > 1,
                Data = userEventDTOs
            };
        }
        public async Task<GetDetailUserEventDTO> GetDetailUserEventDTO(int userId, int orderId)
        {
            // Step 1: Query the Orders where userId and orderId match, including the related Event and EventTickets
            User thisUser = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (thisUser == null)
            {
                return null; // Handle user not found appropriately
            }
            var order = await _context.Order
                .Include(o => o.Event) // Include the related Event
                .Include(o => o.EventTickets) // Include the related EventTickets
                .FirstOrDefaultAsync(o => thisUser.Role == "ADMIN"? o.OrderId == orderId:o.OrderId == orderId && o.UserId == userId);

            // Step 2: Check if the order exists, if not return null or throw an exception
            if (order == null)
            {
                // You can choose to throw an exception or return null
                // throw new NotFoundException("Order not found");
                return null; // Handle appropriately in your service
            }

            // Step 3: Extract the list of ticket codes from the EventTickets related to the order
            var eventTicketCodes = order.EventTickets
                .Where(et => et.OrderId == orderId)
                .Select(et => et.EventTicketCode)
                .ToList();

            // Step 4: Manually map the result to GetDetailUserEventDTO
            var eventDetails = new GetDetailUserEventDTO
            {
                EventId = order.Event.EventId,
                UserName = order.User.FullName,
                UserEmail = order.User.Email,
                Title = order.Event.Title,
                Quantity = order.Quantity,
                TicketPrice = order.Event.TicketPrice,
                EventTicketCodes = eventTicketCodes, // Add the list of ticket codes here
                Status = order.Status, // Assuming order status is part of the Order entity
                Thumbnail = order.Event.Thumbnail,
                Organizer = order.Event.Organizer,
                IsHostOnline = order.Event.IsHostOnline,
                SaleDueDate = order.Event.SaleDueDate,
                MaxTickets = order.Event.MaxTickets,
                CurrentTickets = order.Event.CurrentTickets,
                IsPromoted = order.Event.IsPromoted,
                WebsiteUrl = order.Event.WebsiteUrl,
                StartDate = order.Event.StartDate,
                EndDate = order.Event.EndDate,
                StartTime = order.Event.StartTime,
                EndTime = order.Event.EndTime,
                Continent = order.Event.Continent,
                Country = order.Event.Country,
                Location = order.Event.Location,
                Address = order.Event.Address
            };

            // Step 5: Return the detailed user event DTO
            return eventDetails;
        }


    }
}

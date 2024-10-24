using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using T2305M_API.DTO.Event;
using T2305M_API.Entities;
using T2305M_API.Repositories;
using AutoMapper;


public class EventRepository : IEventRepository
{
    private readonly T2305mApiContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IMapper _mapper;

    public EventRepository(T2305mApiContext context, IWebHostEnvironment env, IMapper mapper)
    {
        _context = context;
        _env = env;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<Event> Data, int TotalItems)> GetEventsAsync(EventQueryParameters queryParameters)
    {
        IQueryable<Event> query = _context.Event;

        // Apply filters
        if (!string.IsNullOrEmpty(queryParameters.Country))
        {
            query = query.Where(a => a.Country == queryParameters.Country);
        }

        if (!string.IsNullOrEmpty(queryParameters.Continent))
        {
            query = query.Where(a => a.Continent == queryParameters.Continent);
        }

        if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
        {
            query = query.Where(a =>
                a.Title.Contains(queryParameters.SearchTerm) ||
                a.Description.Contains(queryParameters.SearchTerm) ||
                a.Organizer.Contains(queryParameters.SearchTerm) ||
                a.User.FullName.Contains(queryParameters.SearchTerm) ||
                a.Location.Contains(queryParameters.SearchTerm) ||
                a.Address.Contains(queryParameters.SearchTerm) ||
                a.Country.Contains(queryParameters.SearchTerm) ||
                a.Continent.Contains(queryParameters.SearchTerm)||
                a.Content.Contains(queryParameters.SearchTerm));
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(queryParameters.SortColumn))
        {
            bool isDescending = queryParameters.SortOrder?.ToLower() == "desc";
            switch (queryParameters.SortColumn.ToLower())
            {
                case "title":
                    query = isDescending ? query.OrderByDescending(a => a.Title) : query.OrderBy(a => a.Title);
                    break;
                case "price":
                    query = isDescending ? query.OrderByDescending(a => a.TicketPrice) : query.OrderBy(a => a.TicketPrice);
                    break;
                default:
                    query = isDescending ? query.OrderByDescending(a => a.StartDate) : query.OrderBy(a => a.StartDate);
                    break;
            }
        }

        // Pagination
        int totalItems = await query.CountAsync();
        var pagedData = await query
            .Skip((queryParameters.Page - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .ToListAsync();

        return (pagedData, totalItems);
    }
    public async Task<Event> GetEventByIdAsync(int eventId)
    {
         var eventInstance = await _context.Event
            .FirstOrDefaultAsync(aa => aa.EventId == eventId);
        return eventInstance;
    }
    public async Task<CreateEventResponseDTO> CreateEventAsync(CreateEventDTO createEventDTO)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                Event newEvent = _mapper.Map<Event>(createEventDTO);

                _context.Event.Add(newEvent);
                await _context.SaveChangesAsync();

                // Commit the transaction if all is successful
                await transaction.CommitAsync();

                // Return the response DTO with the new OrderId

                return new CreateEventResponseDTO
                {
                    EventId = newEvent.EventId,
                    Message = "Event created successfully",
                    Status = newEvent.Status
                };
            }
            catch (DbUpdateException dbEx) // Catch database-specific errors
            {
                await transaction.RollbackAsync(); // Rollback the transaction
                return new CreateEventResponseDTO
                {
                    Message = "Database update failed during event creation: " + dbEx.Message,
                };
            }
            catch (Exception ex) // Catch other exceptions
            {
                await transaction.RollbackAsync(); // Rollback the transaction
                throw ex;
            }
        }
    }
    public async Task<UpdateEventResponseDTO> UpdateEventAsync(UpdateEventDTO updateEventDTO)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // Retrieve the existing event from the database
                var existingEvent = await GetEventByIdAsync(updateEventDTO.EventId);
                if (existingEvent == null)
                {
                    throw new Exception("Event not found");
                }

                // Map the updated fields from the DTO to the existing event entity
                _mapper.Map(updateEventDTO, existingEvent);

                // Update the event in the database
                _context.Event.Update(existingEvent);
                await _context.SaveChangesAsync();

                // Commit the transaction if successful
                await transaction.CommitAsync();

                // Return the response DTO with the updated event details
                return new UpdateEventResponseDTO
                {
                    EventId = existingEvent.EventId,
                    Message = "Event updated successfully",
                    Status = existingEvent.Status
                };
            }
            catch (DbUpdateException dbEx) // Catch database-specific errors
            {
                await transaction.RollbackAsync(); // Rollback the transaction in case of a database error
                throw new Exception("Database update failed during event update: " + dbEx.Message);

            }
            catch (Exception ex) // Catch other types of exceptions
            {
                await transaction.RollbackAsync(); // Rollback the transaction on any other exception
                throw new Exception("An error occurred while updating the event: " + ex.Message, ex);
            }
        }
    }
    public async Task SetEventInactive(int eventId)
    {
        var existingEvent = await GetEventByIdAsync(eventId);
        if (existingEvent == null)
        {
            throw new Exception("Event not found");
        }
        existingEvent.Status = "INACTIVE";

        // Optionally, save the changes to the database if you're using an ORM like Entity Framework
        // For example:
        _context.Event.Update(existingEvent);
        await _context.SaveChangesAsync();
    }




}
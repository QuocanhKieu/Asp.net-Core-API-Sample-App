using T2305M_API.DTO.Event;
using T2305M_API.Entities;

namespace T2305M_API.Repositories
{
    public interface IEventRepository
    {
        Task<(IEnumerable<Event> Data, int TotalItems)> GetEventsAsync(EventQueryParameters queryParameters);
        Task<Event> GetEventByIdAsync(int eventId);
        Task<CreateEventResponseDTO> CreateEventAsync(CreateEventDTO createEventDTO);
        Task<UpdateEventResponseDTO> UpdateEventAsync(UpdateEventDTO updateEventDTO);
        Task SetEventInactive(int eventId);
    }
}

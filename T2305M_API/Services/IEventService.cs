using T2305M_API.DTO.Event;
using T2305M_API.Models;

namespace T2305M_API.Services
{
    public interface IEventService
    {
        Task<PaginatedResult<GetBasicEventDTO>> GetBasicEventDTOsAsync(EventQueryParameters queryParameters);
        Task<GetDetailEventDTO> GetDetailEventDTOByIdAsync(int eventId);
        Task<CreateEventResponseDTO> CreateEventAsync(CreateEventDTO createEventDTO);
        Task<Dictionary<string, List<string>>> ValidateCreateEventDTO(CreateEventDTO createEventDTO);
        Task<UpdateEventResponseDTO> UpdateEventAsync(UpdateEventDTO updateEventDTO);

    }
}

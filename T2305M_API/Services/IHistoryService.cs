using T2305M_API.DTO.History;
using T2305M_API.Models;

namespace T2305M_API.Services
{
    public interface IHistoryService
    {
        Task<PaginatedResult<GetBasicHistoryDTO>> GetBasicHistoryDTOsAsync(HistoryQueryParameters queryParameters);
        Task<GetDetailHistoryDTO> GetDetailHistoryDTOByIdAsync(int historyId);
        Task<Dictionary<string, List<string>>> ValidateCreateHistoryDTO(CreateHistoryDTO createHistoryDTO);
        Task<CreateHistoryResponseDTO> CreateHistoryAsync(CreateHistoryDTO createHistoryDTO);

    }
}

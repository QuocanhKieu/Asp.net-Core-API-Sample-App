using T2305M_API.DTO.History;
using T2305M_API.Entities;

namespace T2305M_API.Repositories
{
    public interface IHistoryRepository
    {
        Task<(IEnumerable<History> Data, int TotalItems)> GetHistoriesAsync(HistoryQueryParameters queryParameters);
        Task<History> GetHistoryByIdAsync(int historyId);
        Task<IEnumerable<History>> GetHistoryByTimeOrderAsync(int timeOrder);
        Task<CreateHistoryResponseDTO> CreateHistoryAsync(CreateHistoryDTO createHistoryDTO);

    }
}

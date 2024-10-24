using T2305M_API.DTO.Culture;
using T2305M_API.Entities;

namespace T2305M_API.Repositories
{
    public interface ICultureRepository
    {
        Task<(IEnumerable<Culture> Data, int TotalItems)> GetCulturesAsync(CultureQueryParameters queryParameters);
        Task<Culture> GetCultureByIdAsync(int cultureId);
        Task<CreateCultureResponseDTO> CreateCultureAsync(CreateCultureDTO createCultureDTO);
    }
}

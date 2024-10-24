using T2305M_API.DTO.Culture;
using T2305M_API.DTO.Culture;
using T2305M_API.Models;

namespace T2305M_API.Services
{
    public interface ICultureService
    {
        Task<PaginatedResult<GetBasicCultureDTO>> GetBasicCultureDTOsAsync(CultureQueryParameters queryParameters);
        Task<GetDetailCultureDTO> GetDetailCultureDTOByIdAsync(int cultureId);
        Task<CreateCultureResponseDTO> CreateCultureAsync(CreateCultureDTO createCultureDTO);

    }
}

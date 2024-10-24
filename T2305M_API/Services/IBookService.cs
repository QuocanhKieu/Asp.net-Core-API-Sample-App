using T2305M_API.DTO.Book;
using T2305M_API.Models;

namespace T2305M_API.Services
{
    public interface IBookService
    {
        Task<PaginatedResult<GetBasicBookDTO>> GetBasicBookDTOsAsync(BookQueryParameters queryParameters);
        Task<CreateBookResponseDTO> CreateBookAsync(CreateBookDTO createBookDTO);
    }
}

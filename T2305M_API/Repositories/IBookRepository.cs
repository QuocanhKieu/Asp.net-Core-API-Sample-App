using T2305M_API.DTO.Book;
using T2305M_API.Entities;

namespace T2305M_API.Repositories
{
    public interface IBookRepository
    {
        Task<(IEnumerable<Book> Data, int TotalItems)> GetBooksAsync(BookQueryParameters queryParameters);
        Task<CreateBookResponseDTO> CreateBookAsync(CreateBookDTO createBookDTO);
    }
}

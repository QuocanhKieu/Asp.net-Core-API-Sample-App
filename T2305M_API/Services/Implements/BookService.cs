using Microsoft.CodeAnalysis.FlowAnalysis;
using T2305M_API.DTO.Culture;
using T2305M_API.DTO.Book;
using T2305M_API.Entities;
using T2305M_API.Models;
using T2305M_API.Repositories;
using T2305M_API.Repositories.Implements;
using T2305M_API.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ICreatorRepository _creatorRepository;
    private readonly IWebHostEnvironment _env;

    public BookService(IBookRepository bookRepository, IWebHostEnvironment env, ICreatorRepository creatorRepository)
    {
        _bookRepository = bookRepository;
        _env = env;
        _creatorRepository = creatorRepository;
    }

    public async Task<PaginatedResult<GetBasicBookDTO>> GetBasicBookDTOsAsync(BookQueryParameters queryParameters)
    {
        var (data, totalItems) = await _bookRepository.GetBooksAsync(queryParameters);

        var basicBookDTOs = new List<GetBasicBookDTO>();

        foreach (var book in data)
        {
            // Fetch the tags associated with the book
            var tagDTOs = book.BookTags?.Select(bt => new TagDTO
            {
                TagId = bt.Tag.TagId,
                Name = bt.Tag.Name
            }).ToList() ?? new List<TagDTO>();
            basicBookDTOs.Add(new GetBasicBookDTO
            {
                BookId = book.BookId,
                Title = book.Title,
                ThumbnailImage = book.ThumbnailImage,
                Author = book.Author,
                IsRecommended = book.IsRecommended,
                Price = book.Price,
                CopiesSold = book.CopiesSold,
                ReleaseDate = book.ReleaseDate,
                Description = book.Description ?? "",
                Tags = tagDTOs // Add the associated tags
            });
        }
        // Calculate total pages
        int totalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize);

        return new PaginatedResult<GetBasicBookDTO>
        {
            TotalItems = totalItems,
            PageSize = queryParameters.PageSize,
            CurrentPage = queryParameters.Page,
            TotalPages = totalPages,
            HasNextPage = queryParameters.Page < totalPages,
            HasPreviousPage = queryParameters.Page > 1,
            Data = basicBookDTOs
        };
    }
    public async Task<CreateBookResponseDTO> CreateBookAsync(CreateBookDTO createBookDTO)
    {
        CreateBookResponseDTO createBookResponseDTO = await _bookRepository.CreateBookAsync(createBookDTO);
        return createBookResponseDTO;
    }
}
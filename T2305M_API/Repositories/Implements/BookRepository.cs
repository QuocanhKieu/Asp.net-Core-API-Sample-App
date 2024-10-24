using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using T2305M_API.DTO.Book;
using T2305M_API.Entities;
using T2305M_API.Repositories;

public class BookRepository : IBookRepository
{
    private readonly T2305mApiContext _context;
    private readonly IWebHostEnvironment _env;

    public BookRepository(T2305mApiContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<(IEnumerable<Book> Data, int TotalItems)> GetBooksAsync(BookQueryParameters queryParameters)
    {
        IQueryable<Book> query = _context.Book
                                         .Include(b => b.BookTags)
                                         .ThenInclude(bt => bt.Tag);  // Include the tags

        // Filter by IsRecommended
        if (queryParameters.IsRecommended)
        {
            query = query.Where(b => b.IsRecommended);
        }
        var searchTerm = queryParameters.SearchTerm?.ToLower().Trim() ?? "";
        // Filter by search term (title, author, etc.)
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(b =>
                b.Title.ToLower().Trim().Contains(searchTerm) ||
                b.Author.ToLower().Trim().Contains(searchTerm) ||
                 (b.Description != null && b.Description.ToLower().Trim().Contains(searchTerm))
            );
        }

        //// Filter by multiple tags
        //if (queryParameters.Tags != null && queryParameters.Tags.Count > 0)
        //{
        //    query = query.Where(b => b.BookTags.Any(bt => queryParameters.Tags.Contains(bt.Tag.Name)));
        //}

        // Apply tag filtering (must contain all tags)
        queryParameters.ParseTags();
        if (queryParameters.Tags != null && queryParameters.Tags.Count > 0)
        {
            query = query.Where(b =>
                b.BookTags.Count(bt => queryParameters.Tags.Contains(bt.Tag.Name)) == queryParameters.Tags.Count);
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(queryParameters.SortColumn))
        {
            bool isDescending = queryParameters.SortOrder?.ToLower() == "desc";
            switch (queryParameters.SortColumn.ToLower())
            {
                case "title":
                    query = isDescending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title);
                    break;
                case "price":
                    query = isDescending ? query.OrderByDescending(b => b.Price) : query.OrderBy(b => b.Price);
                    break;
                case "copiessold":
                    query = isDescending ? query.OrderByDescending(b => b.CopiesSold) : query.OrderBy(b => b.CopiesSold);
                    break;
                default:
                    query = isDescending ? query.OrderByDescending(b => b.ReleaseDate) : query.OrderBy(b => b.ReleaseDate);
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
    public async Task<CreateBookResponseDTO> CreateBookAsync(CreateBookDTO createBookDTO)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // Map the CreateBookDTO properties to the new Book entity
                var newBook = new Book
                {
                    Title = createBookDTO.Title,
                    Author = createBookDTO.Author,
                    ThumbnailImage = createBookDTO.ThumbnailImage,
                    IsRecommended = createBookDTO.IsRecommended,
                    Price = createBookDTO.Price,
                    CopiesSold = createBookDTO.CopiesSold,
                    ReleaseDate = createBookDTO.ReleaseDate,
                    CreatorId = createBookDTO.CreatorId, // Link to the Creator
                    CreatedAt = DateTime.Now, // Set the creation timestamp
                    Description = createBookDTO.Description,
                    IsActive = true // Book is active by default
                };

                // Add the new Book to the DbSet
                _context.Book.Add(newBook);
                await _context.SaveChangesAsync(); // Save the book first to generate BookId

                // If the book has associated tags, create BookTag entries
                if (createBookDTO.TagIds != null && createBookDTO.TagIds.Any(tagId => tagId != 0))
                {
                    foreach (var tagId in createBookDTO.TagIds)
                    {
                        var bookTag = new BookTag
                        {
                            BookId = newBook.BookId, // Reference the newly created book
                            TagId = tagId
                        };
                        _context.BookTag.Add(bookTag); // Add each BookTag
                    }
                    await _context.SaveChangesAsync(); // Save all BookTags
                }

                // Commit the transaction if all is successful
                await transaction.CommitAsync();

                // Return the response DTO with the new BookId
                return new CreateBookResponseDTO
                {
                    BookId = newBook.BookId,
                    IsActive = newBook.IsActive,
                    Message = "Book created successfully"
                };
            }
            catch (DbUpdateException dbEx) // Catch database-specific errors
            {
                await transaction.RollbackAsync(); // Rollback the transaction
                return new CreateBookResponseDTO
                {
                    Message = "Database update failed during book creation: " + dbEx.Message,
                };
            }
            catch (Exception ex) // Catch other exceptions
            {
                await transaction.RollbackAsync(); // Rollback the transaction
                throw ex; // Rethrow the exception after rollback
            }
        }
    }

}
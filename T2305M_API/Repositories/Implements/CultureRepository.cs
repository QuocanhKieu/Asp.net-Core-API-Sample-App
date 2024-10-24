using Microsoft.EntityFrameworkCore;
using T2305M_API.DTO.Culture;
using T2305M_API.Entities;

namespace T2305M_API.Repositories.Implements
{
    public class CultureRepository : ICultureRepository
    {
        private readonly T2305mApiContext _context;

        public CultureRepository(T2305mApiContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Culture> Data, int TotalItems)> GetCulturesAsync(CultureQueryParameters queryParameters)
        {
            IQueryable<Culture> query = _context.Culture;

            // Apply filters
            if (!string.IsNullOrEmpty(queryParameters.Country))
            {
                query = query.Where(a => a.Country == queryParameters.Country);
            }

            if (!string.IsNullOrEmpty(queryParameters.Continent))
            {
                query = query.Where(a => a.Continent == queryParameters.Continent);
            }

            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                query = query.Where(a =>
                    a.Title.Contains(queryParameters.SearchTerm) ||
                    a.Description.Contains(queryParameters.SearchTerm) ||
                    //a.Content.Contains(queryParameters.SearchTerm) ||
                    a.Country.Contains(queryParameters.SearchTerm) ||
                    a.Continent.Contains(queryParameters.SearchTerm) ||
                    a.Creator.Name.Contains(queryParameters.SearchTerm)
                    );
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(queryParameters.SortColumn))
            {
                bool isDescending = queryParameters.SortOrder?.ToLower() == "desc";
                switch (queryParameters.SortColumn.ToLower())
                {
                    case "title":
                        query = isDescending ? query.OrderByDescending(a => a.Title) : query.OrderBy(a => a.Title);
                        break;
                    case "country":
                        query = isDescending ? query.OrderByDescending(a => a.Country) : query.OrderBy(a => a.Country);
                        break;
                    case "continent":
                        query = isDescending ? query.OrderByDescending(a => a.Continent) : query.OrderBy(a => a.Continent);
                        break;
                    default:
                        query = isDescending ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt);
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
        public async Task<Culture> GetCultureByIdAsync(int cultureId)
        {
            return await _context.Culture
                .FirstOrDefaultAsync(aa => aa.CultureId == cultureId);
        }
        public async Task<CreateCultureResponseDTO> CreateCultureAsync(CreateCultureDTO createCultureDTO)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var newCulture = new Culture
                    {
                        Title = createCultureDTO.Title,
                        Continent = createCultureDTO.Continent,
                        Country = createCultureDTO.Country,
                        Description = createCultureDTO.Description,
                        Period = createCultureDTO.Period,
                        ThumbnailImage = createCultureDTO.ThumbnailImage,
                        FileName = createCultureDTO.FileName + ".html",
                        CreatorId = createCultureDTO.CreatorId,
                    };

                    _context.Culture.Add(newCulture);
                    await _context.SaveChangesAsync();

                    // Commit the transaction if all is successful
                    await transaction.CommitAsync();

                    // Return the response DTO with the new OrderId

                    return new CreateCultureResponseDTO
                    {
                        CultureId = newCulture.CultureId,
                        IsActive = newCulture.IsActive,
                        Message = "Culture created successfully"
                    };
                }
                catch (DbUpdateException dbEx) // Catch database-specific errors
                {
                    await transaction.RollbackAsync(); // Rollback the transaction
                    return new CreateCultureResponseDTO
                    {
                        Message = "Database update failed during culture creation: " + dbEx.Message,
                    };
                }
                catch (Exception ex) // Catch other exceptions
                {
                    await transaction.RollbackAsync(); // Rollback the transaction
                    throw ex;
                }
            }
        }

    }
}

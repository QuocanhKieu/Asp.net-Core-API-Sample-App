using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using T2305M_API.DTO.History;
using T2305M_API.Entities;

namespace T2305M_API.Repositories.Implements
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly T2305mApiContext _context;

        public HistoryRepository(T2305mApiContext context)
        {
            _context = context;
        }
        public async Task<(IEnumerable<History> Data, int TotalItems)> GetHistoriesAsync(HistoryQueryParameters queryParameters)
        {
            IQueryable<History> query = _context.History;

            // Apply filters
            if (!string.IsNullOrEmpty(queryParameters.Country))
            {
                query = query.Where(a => a.Country == queryParameters.Country.Trim());
            }
            if (!string.IsNullOrEmpty(queryParameters.Continent))
            {
                query = query.Where(a => a.Continent == queryParameters.Continent.Trim());
            }
            if (!string.IsNullOrEmpty(queryParameters.Period))
            {
                query = query.Where(a => a.Period == queryParameters.Period.Trim());
            }
            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                query = query.Where(a =>
                    a.Title.Contains(queryParameters.SearchTerm) ||
                    a.Description.Contains(queryParameters.SearchTerm) ||
                    a.Period.Contains(queryParameters.Period) ||
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
                        query = isDescending ? query.OrderByDescending(a => a.TimeOrder) : query.OrderBy(a => a.TimeOrder);
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
        public async Task<History> GetHistoryByIdAsync(int historyId)
        {
            return await _context.History
                .FirstOrDefaultAsync(aa => aa.HistoryId == historyId);
        }

        public async Task<IEnumerable<History>> GetHistoryByTimeOrderAsync(int timeOrder)
        {
            // Query to find all history records that match the timeOrder
            var historyList = await _context.History
                .Where(h => h.TimeOrder == timeOrder)
                .ToListAsync();

            // Return the list of history records
            return historyList;
        }

        public async Task<CreateHistoryResponseDTO> CreateHistoryAsync(CreateHistoryDTO createHistoryDTO)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var newHistory = new History
                    {
                        Title = createHistoryDTO.Title,
                        Continent = createHistoryDTO.Continent,
                        Country = createHistoryDTO.Country,
                        Description = createHistoryDTO.Description,
                        Period = createHistoryDTO.Period,
                        ThumbnailImage = createHistoryDTO.ThumbnailImage,
                        TimeOrder = createHistoryDTO.TimeOrder,
                        FileName = createHistoryDTO.FileName + ".html",
                        CreatorId = createHistoryDTO.CreatorId,
                    };

                    _context.History.Add(newHistory);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new CreateHistoryResponseDTO
                    {
                        HistoryId = newHistory.HistoryId,
                        IsActive = newHistory.IsActive,
                        Message = "History created successfully"
                    };
                }
                catch (DbUpdateException dbEx) // Catch database-specific errors
                {
                    await transaction.RollbackAsync(); // Rollback the transaction
                    return new CreateHistoryResponseDTO
                    {
                        Message = "Database update failed during history creation: " + dbEx.Message,
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

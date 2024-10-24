using Microsoft.EntityFrameworkCore;
using T2305M_API.Entities;
using System.Linq;
using System.Threading.Tasks;
using T2305M_API.Models;
using T2305M_API.DTO.Culture;
using T2305M_API.DTO.Exhibition;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using T2305M_API.DTO.History;
using T2305M_API.DTO.Search;
using Microsoft.CodeAnalysis.Elfie.Extensions;

namespace T2305M_API.Services.Implements
{
    public class SearchServiceImpl
    {
        private readonly T2305mApiContext _context;

        public SearchServiceImpl(T2305mApiContext context)
        {
            _context = context;

        }
        public async Task<PaginatedResult<SearchResultDTO>> SearchArticlesAsync(SearchParameters searchParameters)
        {
            var searchTerm = searchParameters.SearchTerm?.ToLower();
            var type = searchParameters.Type?.ToLower();
            var pageNumber = searchParameters.Page;
            var pageSize = searchParameters.PageSize;
            var country = searchParameters.Country;
            var continent = searchParameters.Continent;


            // Query for each entity type
            var historyQuery = _context.History.Include(h => h.Creator).AsQueryable();  // Include Creator
            var cultureQuery = _context.Culture.Include(c => c.Creator).AsQueryable();  // Include Creator


            if (!string.IsNullOrEmpty(country))
            {
                historyQuery = historyQuery.Where(e => e.Country.ToLower().Contains(country.ToLower().Trim()));

                cultureQuery = cultureQuery.Where(n => n.Country.ToLower().Contains(country.ToLower().Trim()));
            }
            if (!string.IsNullOrEmpty(continent))
            {
                historyQuery = historyQuery.Where(e => e.Continent.ToLower().Contains(continent.ToLower().Trim()));

                cultureQuery = cultureQuery.Where(n => n.Continent.ToLower().Contains(continent.ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                historyQuery = historyQuery.Where(e => e.Title.ToLower().Contains(searchTerm)
                                                        || e.Description.ToLower().Contains(searchTerm)
                                                        || e.Creator.Name.ToLower().Contains(searchTerm)); // Search by CreatorName
                cultureQuery = cultureQuery.Where(n => n.Title.ToLower().Contains(searchTerm)
                                                        || n.Description.ToLower().Contains(searchTerm)
                                                        || n.Creator.Name.ToLower().Contains(searchTerm)); // Search by CreatorName
            }
            //else
            //{
            //     return new PaginatedResult<SearchResultDTO>
            //    {
            //        Data = new List<SearchResultDTO>(),
            //        TotalItems = 0,
            //        PageSize = pageSize,
            //        CurrentPage = pageNumber,
            //        TotalPages = 0,
            //        HasNextPage = false,
            //        HasPreviousPage = false
            //    };
            //}

            if (!string.IsNullOrEmpty(type))
            {
                if (type == "culture")
                {
                    historyQuery = historyQuery.Take(0); // No History Results
                }
                else if (type == "history")
                {
                    cultureQuery = cultureQuery.Take(0); // No Culture Results
                }
            }

            var historyResults = historyQuery
                .Select(a => new SearchResultDTO
                {
                    Id = a.HistoryId,
                    ThumbnailImage = a.ThumbnailImage,
                    Title = a.Title,
                    Description = a.Description,
                    Type = "History",
                    CreatorName = a.Creator.Name,
                    CreatedAt = a.CreatedAt
                });

            var cultureResults = cultureQuery
                .Select(n => new SearchResultDTO
                {
                    Id = n.CultureId,
                    ThumbnailImage = n.ThumbnailImage,
                    Title = n.Title,
                    Description = n.Description,
                    Type = "Culture",
                    CreatorName = n.Creator.Name,
                    CreatedAt = n.CreatedAt
                });

            var combinedQuery = historyResults.Union(cultureResults);

            if (!string.IsNullOrEmpty(searchParameters.SortColumn))
            {
                bool isDescending = searchParameters.SortOrder?.ToLower() == "desc";
                switch (searchParameters.SortColumn.ToLower())
                {
                    case "title":
                        combinedQuery = isDescending
                            ? combinedQuery.OrderByDescending(item => item.Title)
                            : combinedQuery.OrderBy(item => item.Title);
                        break;
                    default:
                        combinedQuery = isDescending
                            ? combinedQuery.OrderByDescending(item => item.CreatedAt)
                            : combinedQuery.OrderBy(item => item.CreatedAt);
                        break;
                }
            }

            var pagedResults = combinedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalItems = combinedQuery.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            bool hasNextPage = pageNumber < totalPages;
            bool hasPreviousPage = pageNumber > 1;

            return new PaginatedResult<SearchResultDTO>
            {
                Data = pagedResults,
                TotalItems = totalItems,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage
            };
        }


    }
}


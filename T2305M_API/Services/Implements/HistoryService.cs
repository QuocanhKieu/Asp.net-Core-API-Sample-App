using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using T2305M_API.DTO.History;
using T2305M_API.Entities;
using T2305M_API.Models;
using T2305M_API.Repositories;

namespace T2305M_API.Services.Implements
{
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly ICreatorRepository _creatorRepository;

        private readonly IWebHostEnvironment _env;

        public HistoryService(IHistoryRepository historyRepository, IWebHostEnvironment env, ICreatorRepository creatorRepository)
        {
            _historyRepository = historyRepository;
            _env = env;
            _creatorRepository = creatorRepository;
        }

        public async Task<PaginatedResult<GetBasicHistoryDTO>> GetBasicHistoryDTOsAsync(HistoryQueryParameters queryParameters)
        {
            var (data, totalItems) = await _historyRepository.GetHistoriesAsync(queryParameters);
            // Initialize a list for the DTOs
            var basicHistoryDTOs = new List<GetBasicHistoryDTO>();

            // Loop through each history and populate CreatorName asynchronously
            foreach (var a in data)
            {
                var creator = await _creatorRepository.GetCreatorByIdAsync(a.CreatorId);

                basicHistoryDTOs.Add(new GetBasicHistoryDTO
                {
                    HistoryId = a.HistoryId,
                    ThumbnailImage = a.ThumbnailImage,
                    Title = a.Title,
                    Description = a.Description,
                    CreatorName = creator.Name
                });
            }

            // Calculate total pages
            int totalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize);

            return new PaginatedResult<GetBasicHistoryDTO>
            {
                TotalItems = totalItems,
                PageSize = queryParameters.PageSize,
                CurrentPage = queryParameters.Page,
                TotalPages = totalPages,
                HasNextPage = queryParameters.Page < totalPages,
                HasPreviousPage = queryParameters.Page > 1,
                Data = basicHistoryDTOs
            };
        }

        public async Task<GetDetailHistoryDTO> GetDetailHistoryDTOByIdAsync(int historyId)
        {
            var history = await _historyRepository.GetHistoryByIdAsync(historyId);

            if (history == null)
            {
                return null; // Or throw an appropriate exception

            }
            // Define the path to your HTML file
            var filePath = Path.Combine(_env.WebRootPath, "content/History", history.FileName);

            string htmlContent = "";
            if (System.IO.File.Exists(filePath))
            {
                htmlContent = System.IO.File.ReadAllText(filePath);
            }

            var creator = await _creatorRepository.GetCreatorByIdAsync(history.CreatorId);

            // Map the nationalEventArticle to NationalEventArticleDetailDto
            var detailHistoryDTO = new GetDetailHistoryDTO
            {
                HistoryId = historyId,
                Title = history.Title,
                Content = htmlContent,
                Country = history.Country,
                Continent = history.Continent,
                Period = history.Period,
                CreatorName = creator.Name,
            };

            return detailHistoryDTO;
        }

        public async Task<Dictionary<string, List<string>>> ValidateCreateHistoryDTO(CreateHistoryDTO createHistoryDTO)
        {
            var errors = new Dictionary<string, List<string>>();

            //Validate CustomerId
            if (createHistoryDTO.CreatorId == 0)
            {
                AddError(errors, "CreatorId", "CreatorId is required.");
            }
            else if (await _creatorRepository.GetCreatorByIdAsync(createHistoryDTO.CreatorId) == null)
            {
                AddError(errors, "CreatorId", "CreatorId not exists.");
            }
            if (createHistoryDTO.Continent.IsNullOrEmpty())
            {
                AddError(errors, "Continent", "Continent is required.");
            }
            if (createHistoryDTO.Country.IsNullOrEmpty())
            {
                AddError(errors, "Country", "Country is required.");
            }
            if (createHistoryDTO.Description.IsNullOrEmpty())
            {
                AddError(errors, "Description", "Description is required.");
            }
            if (createHistoryDTO.Period.IsNullOrEmpty())
            {
                AddError(errors, "Period", "Period is required.");
            }
            if (createHistoryDTO.ThumbnailImage.IsNullOrEmpty())
            {
                AddError(errors, "ThumbnailImage", "ThumbnailImage is required.");
            }
            if (createHistoryDTO.TimeOrder == 0)
            {
                AddError(errors, "TimeOrder", "TimeOrder is required.");
            }
            else if (await _historyRepository.GetHistoryByTimeOrderAsync(createHistoryDTO.TimeOrder) != null)
            {
                var histories = await _historyRepository.GetHistoryByTimeOrderAsync(createHistoryDTO.TimeOrder);
                foreach(var history in histories)
        {
                    if (history.Country == createHistoryDTO.Country)
                    {
                        AddError(errors, "TimeOrder", "A history with the same TimeOrder and Country already exists.");
                        break; // Exit the loop once a match is found
                    }
                }
            }
            if (createHistoryDTO.Title.IsNullOrEmpty())
            {
                AddError(errors, "Title", "Title is required.");
            }

            // Validate Items
            //if (orderDto.Items == null || !orderDto.Items.Any())
            //{
            //    AddError(errors, "Items", "At least one item must be included in the order.");
            //}
            //else
            //{
            //    // Validate each item in the list
            //    for (int i = 0; i < orderDto.Items.Count; i++)
            //    {
            //        var item = orderDto.Items[i];

            //        if (item.ProductId <= 0)
            //        {
            //            AddError(errors, $"Items[{i}].ProductId", "Product ID must be greater than 0.");
            //        }

            //        if (item.Quantity <= 0)
            //        {
            //            AddError(errors, $"Items[{i}].Quantity", "Quantity must be greater than 0.");
            //        }
            //    }
            //}

            return errors.Count > 0 ? errors : null;
        }

        // Helper method to add errors
        private static void AddError(Dictionary<string, List<string>> errors, string key, string errorMessage)
        {
            if (!errors.ContainsKey(key))
            {
                errors[key] = new List<string>();
            }
            errors[key].Add(errorMessage);
        }

        // This method handles the creation of an order
        public async Task<CreateHistoryResponseDTO> CreateHistoryAsync(CreateHistoryDTO createHistoryDTO)
        {
            CreateHistoryResponseDTO createHistoryResponseDTO = await _historyRepository.CreateHistoryAsync(createHistoryDTO);
            return createHistoryResponseDTO;
        }
    }
}

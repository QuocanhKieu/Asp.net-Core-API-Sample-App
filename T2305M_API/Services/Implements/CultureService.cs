using T2305M_API.DTO.Culture;
using T2305M_API.Models;
using T2305M_API.Repositories;
namespace T2305M_API.Services.Implements
{
    public class CultureService : ICultureService
    {
        private readonly ICultureRepository _cultureRepository;
        private readonly ICreatorRepository _creatorRepository;
        private readonly IWebHostEnvironment _env;

        public CultureService(ICultureRepository cultureRepository, IWebHostEnvironment env, ICreatorRepository creatorRepository)
        {
            _cultureRepository = cultureRepository;
            _env = env;
            _creatorRepository = creatorRepository;
        }

        public async Task<PaginatedResult<GetBasicCultureDTO>> GetBasicCultureDTOsAsync(CultureQueryParameters queryParameters)
        {
            var (data, totalItems) = await _cultureRepository.GetCulturesAsync(queryParameters);

            var basicCultureDTOs = new List<GetBasicCultureDTO>();

            foreach (var a in data)
            {
                var creator = await _creatorRepository.GetCreatorByIdAsync(a.CreatorId);

                basicCultureDTOs.Add(new GetBasicCultureDTO
                {
                    CultureId = a.CultureId,
                    ThumbnailImage = a.ThumbnailImage,
                    Title = a.Title,
                    Description = a.Description,
                    CreatorName = creator.Name,
                });
            }
            // Calculate total pages
            int totalPages = (int)Math.Ceiling((double)totalItems / queryParameters.PageSize);

            return new PaginatedResult<GetBasicCultureDTO>
            {
                TotalItems = totalItems,
                PageSize = queryParameters.PageSize,
                CurrentPage = queryParameters.Page,
                TotalPages = totalPages,
                HasNextPage = queryParameters.Page < totalPages,
                HasPreviousPage = queryParameters.Page > 1,
                Data = basicCultureDTOs
            };
        }

        public async Task<GetDetailCultureDTO> GetDetailCultureDTOByIdAsync(int cultureId)
        {
            var culture = await _cultureRepository.GetCultureByIdAsync(cultureId);

            if (culture == null)
            {
                return null; // Or throw an appropriate exception

            }
            // Define the path to your HTML file
            var filePath = Path.Combine(_env.WebRootPath, "content/Culture", culture.FileName);

            string htmlContent = "Updating...";
            if (System.IO.File.Exists(filePath))
            {
                htmlContent = System.IO.File.ReadAllText(filePath);
            }

            var creator = await _creatorRepository.GetCreatorByIdAsync(culture.CreatorId);

            // Map the nationalEventArticle to NationalEventArticleDetailDto
            var detailCultureDTO = new GetDetailCultureDTO
            {
                CultureId = cultureId,
                Title = culture.Title,
                Content = htmlContent,
                Country = culture.Country,
                Continent = culture.Continent,
                Period = culture.Period,
                CreatorName = creator.Name
            };

            return detailCultureDTO;
        }

        public async Task<CreateCultureResponseDTO> CreateCultureAsync(CreateCultureDTO createCultureDTO)
        {
            CreateCultureResponseDTO createCultureResponseDTO = await _cultureRepository.CreateCultureAsync(createCultureDTO);
            return createCultureResponseDTO;
        }
    }
}

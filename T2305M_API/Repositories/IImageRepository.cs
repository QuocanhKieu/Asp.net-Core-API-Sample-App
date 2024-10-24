using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T2305M_API.DTO;

namespace T2305M_API.Repositories
{
    public interface IImageRepository
    {
        Task<List<GetBasicImageDTO>> GetImagesForEntity(int entityId , string entityType);
        Task<CreateImagesResponseDTO> CreateImagesForEntity(CreateImagesDTO createImagesDTO);


    }
}

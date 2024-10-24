using Microsoft.EntityFrameworkCore;
using T2305M_API.DTO.Culture;
using T2305M_API.DTO.History;
using T2305M_API.Entities;

namespace T2305M_API.Repositories
{
    public interface ICreatorRepository
    {
        Task<Creator> GetCreatorByIdAsync(int creatorId); 
    }
}

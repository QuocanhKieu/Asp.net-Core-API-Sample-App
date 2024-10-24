using Microsoft.EntityFrameworkCore;
using T2305M_API.Entities;

namespace T2305M_API.Repositories.Implements
{
    public class CreatorRepository: ICreatorRepository
    {
        private readonly T2305mApiContext _context;

        public CreatorRepository(T2305mApiContext context)
        {
            _context = context;
        }
        public async Task<Creator> GetCreatorByIdAsync(int creatorId)
        {
            return await _context.Creator
                            .FirstOrDefaultAsync(aa => aa.CreatorId == creatorId);
        }

    }
}

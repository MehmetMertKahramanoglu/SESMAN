using Microsoft.EntityFrameworkCore;
using SESMAN.Domain.Entities;
using SESMAN.Domain.ReposInterfaces;

namespace SESMAN.Infrastructure.Repositories
{
    public class CollectionRepository : BaseRepository<Collection>, ICollectionRepository
    {
        public CollectionRepository(AppDbContext context) : base(context)
        {
        }
        public new async Task<List<Collection>> GetAllAsync()
        {
            return await _context.Collections
                .Include(c => c.SavedRequests)
                .ThenInclude(sc => sc.SavedRequestHeaders)
                    .Include(c => c.SavedRequests)
                .ThenInclude(sr => sr.SavedRequestParameters)
                .ToListAsync(); // ToListAsync zaten geriye List döner
        }
    }
}
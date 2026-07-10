using Microsoft.EntityFrameworkCore;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.Entities;
using SESMAN.Domain.ReposInterfaces;

namespace SESMAN.Infrastructure.Repositories
{
    public class RequestLogRepository : BaseRepository<RequestLog>, IRequestLogRepository
    {
        public RequestLogRepository(AppDbContext context) : base(context)
        {

        }

        public new async Task<RequestLog?> GetByIdAsync(Guid id) 
        {
            return await _context.RequestLogs
                .Include(x => x.RequestHeaders)
                .Include(x => x.RequestParameters)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
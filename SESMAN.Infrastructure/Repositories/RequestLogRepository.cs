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

        //bu kısım id ile 1 veri çekmek için
        public new async Task<RequestLog?> GetByIdAsync(Guid id) 
        {
            return await _context.RequestLogs
                .Include(x => x.RequestHeaders)
                .Include(x => x.RequestParameters)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        //bu kısım sınırlandırma olmadan bütün veriyi çekmek için.
        public new async Task<IEnumerable<RequestLog>> GetAllAsync()
        {
            return await _context.RequestLogs
                .Include(x => x.RequestHeaders)
                .Include(x => x.RequestParameters)
                .Include(x => x.Response)
                .ThenInclude(r => r.ResponseHeaders)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        //burada history tarafındaki gösterimi sınırlandırmak için tanımlama yaptım
        public async Task<IEnumerable<RequestLog>> GetPagedHistoryAsync(int page, int pageSize)
        {
            return await _context.RequestLogs
                .Include(x => x.RequestHeaders)
                .Include(x => x.RequestParameters)
                .Include(x => x.Response)
                    .ThenInclude(r => r.ResponseHeaders)
                 .OrderByDescending(x => x.CreatedAt)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }
    }
}
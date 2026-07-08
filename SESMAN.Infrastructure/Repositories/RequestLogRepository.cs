using Microsoft.EntityFrameworkCore;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.Entities;

namespace SESMAN.Infrastructure.Repositories
{
    public class RequestLogRepository : IRequestLogRepository
    {
        private readonly AppDbContext _context;

        //Veritabanı bağlantısı kurdum.
        public RequestLogRepository(AppDbContext context)
        {
            _context = context;
        }

        //READ ALL 
        public async Task<List<RequestLog>> GetAllAsync()
        {
            return await _context.RequestLogs.Include(x => x.RequestHeaders)
        .Include(x => x.RequestParameters)
        .ToListAsync();
        }

        //READ BY ID 
        public async Task<RequestLog?> GetByIdAsync(Guid id)
        {
            return await _context.RequestLogs
        .Include(x => x.RequestHeaders)
        .Include(x => x.RequestParameters)
        .FirstOrDefaultAsync(x => x.Id == id); // FindAsync yerine FirstOrDefaultAsync kullandık çünkü Include ile Find yan yana çalışmıyormuş.
        }

        //CREATE 
        public async Task AddAsync(RequestLog entity)
        {
            await _context.RequestLogs.AddAsync(entity);
            await _context.SaveChangesAsync(); //Değişiklikleri PostgreSQL'e kaydet
        }

        //UPDATE 
        public async Task UpdateAsync(RequestLog entity)
        {
            _context.RequestLogs.Update(entity);
            await _context.SaveChangesAsync();
        }

        //DELETE 
        public async Task DeleteAsync(Guid id)
        {
            // Önce silinecek kaydı bul
            var entity = await _context.RequestLogs.FindAsync(id);
            if (entity != null)
            {
                _context.RequestLogs.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
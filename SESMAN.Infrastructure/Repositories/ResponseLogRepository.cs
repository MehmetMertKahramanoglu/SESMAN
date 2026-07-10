using Microsoft.EntityFrameworkCore;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.Entities;
using SESMAN.Domain.ReposInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Infrastructure.Repositories
{
    public class ResponseLogRepository : BaseRepository<ResponseLog>, IResponseLogRepository
    {
        public ResponseLogRepository(AppDbContext context) : base(context)
        {
        }

        public new async Task<List<ResponseLog>> GetAllAsync()
        {
            return await _context.ResponseLogs
                .Include(x => x.ResponseHeaders)
                .ToListAsync();
        }

        public new async Task<ResponseLog?> GetByIdAsync(Guid id)
        {
            return await _context.ResponseLogs
                .Include(x => x.ResponseHeaders)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SESMAN.Domain.Entities;
using SESMAN.Domain.ReposInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Infrastructure.Repositories
{
    public class SavedRequestRepository: BaseRepository<SavedRequest>, ISavedRequestRepository
    {
        public SavedRequestRepository(AppDbContext context) : base(context)
        {
        }
        public new async Task<List<SavedRequest>> GetAllAsync()
        {
            return await _context.SavedRequests
                .Include(s => s.SavedRequestHeaders).Include(s => s.SavedRequestParameters)
                .ToListAsync(); // ToListAsync zaten geriye List döner
        }
    }
}

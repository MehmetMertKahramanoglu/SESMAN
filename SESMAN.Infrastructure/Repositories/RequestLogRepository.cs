using Microsoft.EntityFrameworkCore;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.Entities;

namespace SESMAN.Infrastructure.Repositories
{
    public class RequestLogRepository : BaseRepository<RequestLog>, IRequestLogRepository
    {
        public RequestLogRepository(AppDbContext context) : base(context)
        {
        }
    }
}
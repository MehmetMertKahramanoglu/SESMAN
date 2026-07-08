using SESMAN.Domain.Entities;

namespace SESMAN.Application.Interfaces
{
    public interface IRequestLogRepository
    {
        // 1. READ ALL 
        Task<List<RequestLog>> GetAllAsync();

        // 2. READ BY ID 
        Task<RequestLog?> GetByIdAsync(Guid id);

        // 3. CREATE 
        Task AddAsync(RequestLog entity);

        // 4. UPDATE
        Task UpdateAsync(RequestLog entity);

        // 5. DELETE
        Task DeleteAsync(Guid id);
    }
}
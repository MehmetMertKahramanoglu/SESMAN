using SESMAN.Application.DTOs;

namespace SESMAN.Application.Interfaces
{
    public interface IRequestLogService
    {
        Task<List<RequestLogDto>> GetAllAsync();
        Task<RequestLogDto?> GetByIdAsync(Guid id);
        Task CreateAsync(CreateLogDto dto);
        Task UpdateAsync(Guid id, UpdateLogDto dto);
        Task PatchAsync(Guid id, UpdateLogDto dto);
        Task DeleteAsync(Guid id);
    }
}
using SESMAN.Application.DTOs;

namespace SESMAN.Application.Interfaces
{
    public interface IRequestLogService : IBaseService<RequestLogDto, CreateLogDto, UpdateLogDto>
    {
        // BaseService'te olmayan PATCH kısmını ekledim
        Task PatchAsync(Guid id, UpdateLogDto dto);
    }
}
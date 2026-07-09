using SESMAN.Application.DTOs;

namespace SESMAN.Application.Interfaces
{
    public interface IRequestLogService : IBaseService<RequestLogDto, CreateLogDto, UpdateLogDto>
    {
        Task PatchAsync(Guid id, UpdateLogDto dto);
    }
}
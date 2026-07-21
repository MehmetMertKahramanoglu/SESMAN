using SESMAN.Application.DTOs;

namespace SESMAN.Application.Interfaces
{
    public interface IRequestLogService : IBaseService<RequestLogDto, CreateRequestLogDto, UpdateRequestLogDto>
    {
        Task PatchAsync(Guid id, UpdateRequestLogDto dto);

        Task<IEnumerable<RequestLogDto>> GetPagedHistoryAsync(int page, int pageSize);
    }
}
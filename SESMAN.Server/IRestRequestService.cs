using SESMAN.Application.DTOs;

namespace SESMAN.Server
{
    public interface IRestRequestService
    {
        // Vue'dan gelen Create DTO'sunu alır, işlemi yapar ve ekranda göstermek üzere dolu DTO'yu döner
        Task<RequestLogDto> ExecuteAndSaveRequestAsync(CreateRequestLogDto dto);
    }
}

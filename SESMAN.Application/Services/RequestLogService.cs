using SESMAN.Application.DTOs;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.Entities;
using SESMAN.Domain.Enums;

namespace SESMAN.Application.Services
{ //ID LERİ KENDİ OTOMATİK OLUŞTURUYOR BENİM YAPMAMA GEREK YOK İDE YAPMIYOR POSTGRE KISMI YAPIYOR BUNU !!!
    public class RequestLogService : BaseService<RequestLog, RequestLogDto, CreateLogDto, UpdateLogDto>, IRequestLogService
    {
        public RequestLogService(IRequestLogRepository repository) : base(repository)
        {
        }
        //şimdilik manuel mapping yapıyorum AUTOMAPPİNG ARAŞTIRIP DÜZELT!
        protected override RequestLogDto MapToDto(RequestLog entity)
        {
            return new RequestLogDto
            {
                Id = entity.Id,
                Url = entity.Url,
                Method = entity.Method.ToString(),
                RequestBody = entity.Body,
            };
        }

        protected override RequestLog MapToEntity(CreateLogDto createDto)
        {
            return new RequestLog
            {
                Url = createDto.Url ?? string.Empty,
                Method = Enum.Parse<HttpMethodType>(createDto.Method ?? "GET", true),

                RequestHeaders = createDto.Headers?.Select(h => new RequestHeader { Key = h.Key, Value = h.Value }).ToList() ?? new(),
                RequestParameters = createDto.Parameters?.Select(p => new RequestParameter { Key = p.Key, Value = p.Value }).ToList() ?? new()
            };
        }

        protected override void MapUpdateDtoToEntity(UpdateLogDto updateDto, RequestLog entity)
        {
            entity.Url = updateDto.Url ?? entity.Url;
            entity.Method = Enum.Parse<HttpMethodType>(updateDto.Method ?? entity.Method.ToString(), true);
            
        }
        public async Task PatchAsync(Guid id, UpdateLogDto dto)
        {
            var existingLog = await _repository.GetByIdAsync(id);
            if (existingLog != null)
            {
                if (!string.IsNullOrEmpty(dto.Url)) existingLog.Url = dto.Url;
                if (!string.IsNullOrEmpty(dto.Method)) existingLog.Method = Enum.Parse<HttpMethodType>(dto.Method, true);
                if (!string.IsNullOrEmpty(dto.RequestBody)) existingLog.Body = dto.RequestBody;

                if (dto.Headers != null && dto.Headers.Any())
                {
                    existingLog.RequestHeaders = dto.Headers.Select(h => new RequestHeader { Id = Guid.NewGuid(), Key = h.Key, Value = h.Value }).ToList();
                }

                if (dto.Parameters != null && dto.Parameters.Any())
                {
                    existingLog.RequestParameters = dto.Parameters.Select(p => new RequestParameter { Id = Guid.NewGuid(), Key = p.Key, Value = p.Value }).ToList();
                }

                await _repository.UpdateAsync(existingLog);
            }
        }
    }
}
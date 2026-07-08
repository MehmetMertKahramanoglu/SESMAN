using SESMAN.Application.DTOs;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.Entities;
using SESMAN.Domain.Enums;

namespace SESMAN.Application.Services
{
    public class RequestLogService : IRequestLogService
    {
        private readonly IRequestLogRepository _repository;

        public RequestLogService(IRequestLogRepository repository)
        {
            _repository = repository;
        }

        // 1. GET ALL
        public async Task<List<RequestLogDto>> GetAllAsync()
        {
            var logs = await _repository.GetAllAsync();
            return logs.Select(x => new RequestLogDto
            {
                Id = x.Id,
                Url = x.Url,
                Method = x.Method.ToString(),
                RequestBody = x.Body,
                CreatedAt = x.CreatedAt,

                // Veritabanından gelen alt tabloları DTO'ya çeviriyoruz
                Headers = x.RequestHeaders.Select(h => new HeaderDto { Key = h.Key, Value = h.Value }).ToList(),
                Parameters = x.RequestParameters.Select(p => new ParameterDto { Key = p.Key, Value = p.Value }).ToList()
            }).ToList();
        }

        // 2. GET BY ID
        public async Task<RequestLogDto?> GetByIdAsync(Guid id)
        {
            var log = await _repository.GetByIdAsync(id);
            if (log == null) return null;

            return new RequestLogDto
            {
                Id = log.Id,
                Url = log.Url,
                Method = log.Method.ToString(),
                RequestBody = log.Body,
                CreatedAt = log.CreatedAt,

                Headers = log.RequestHeaders.Select(h => new HeaderDto { Key = h.Key, Value = h.Value }).ToList(),
                Parameters = log.RequestParameters.Select(p => new ParameterDto { Key = p.Key, Value = p.Value }).ToList()
            };
        }

        //POST
        public async Task CreateAsync(CreateLogDto dto)
        {
            var newLog = new RequestLog
            {
                Id = Guid.NewGuid(),
                Url = dto.Url ?? string.Empty,
                Method = Enum.Parse<HttpMethodType>(dto.Method ?? "GET", true),
                Body = dto.RequestBody,
                CreatedAt = DateTime.UtcNow,
                

                // DTO ile gelen listeleri veritabanı Entity sınıflarına çeviriyoruz
                RequestHeaders = dto.Headers?.Select(h => new RequestHeader { Id = Guid.NewGuid(), Key = h.Key, Value = h.Value }).ToList() ?? new(),
                RequestParameters = dto.Parameters?.Select(p => new RequestParameter { Id = Guid.NewGuid(), Key = p.Key, Value = p.Value }).ToList() ?? new()
            };

            // EF Core Ana nesneyi kaydettiğinde içindeki Headers ve Parameters listelerin otomatik olarak algılar ve gidip kendi alt tablolarına Foreign Key ile beraber yazar.
            await _repository.AddAsync(newLog);
        }

        // PUT 
        public async Task UpdateAsync(Guid id, UpdateLogDto dto)
        {
            var existingLog = await _repository.GetByIdAsync(id);
            if (existingLog != null)
            {
                existingLog.Url = dto.Url ?? existingLog.Url;
                existingLog.Method = Enum.Parse<HttpMethodType>(dto.Method ?? existingLog.Method.ToString(), true);
                existingLog.Body = dto.RequestBody ?? existingLog.Body;

                // Eski başlıkları ve parametreleri temizleyip yenilerini ekliyoruz
                existingLog.RequestHeaders = dto.Headers?.Select(h => new RequestHeader { Id = Guid.NewGuid(), Key = h.Key, Value = h.Value }).ToList() ?? new();
                existingLog.RequestParameters = dto.Parameters?.Select(p => new RequestParameter { Id = Guid.NewGuid(), Key = p.Key, Value = p.Value }).ToList() ?? new();

                await _repository.UpdateAsync(existingLog);
            }
        }

        // PATCH 
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

        // DELETE
        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
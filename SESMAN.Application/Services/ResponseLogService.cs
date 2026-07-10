using AutoMapper;
using SESMAN.Application.DTOs;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.Entities;
using SESMAN.Domain.ReposInterfaces;

namespace SESMAN.Application.Services
{
    public class ResponseLogService : BaseService<ResponseLog, ResponseLogDto, CreateResponseLogDto, UpdateResponseLogDto>, IResponseLogService
    {
        public ResponseLogService(IResponseLogRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        // PUT İŞLEMİ
        public override async Task UpdateAsync(Guid id, UpdateResponseLogDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _mapper.Map(dto, entity); //BURADA ANA TABLOYU GÜNCELLEDİM İÇİNDE HEADERS İGNORE OLDUĞU İÇİN BAĞLANTIYI KESMEMİŞ OLDU

                //HEADER İÇİN AKILLI GÜNCELLEME
               
                    var incomingHeaderIds = dto.ResponseHeaders
                        .Where(h => h.Id != Guid.Empty)
                        .Select(h => h.Id)
                        .ToList();

                    
                    var headersToRemove = entity.ResponseHeaders.Where(h => !incomingHeaderIds.Contains(h.Id)).ToList();
                    foreach (var h in headersToRemove)
                    {
                        entity.ResponseHeaders.Remove(h);
                    }

                    foreach (var headerDto in dto.ResponseHeaders)
                    {
                        if (headerDto.Id != Guid.Empty)
                        {
                            var existingHeader = entity.ResponseHeaders.FirstOrDefault(h => h.Id == headerDto.Id);
                            if (existingHeader != null)
                            {
                                _mapper.Map(headerDto, existingHeader);
                            }
                        }
                        else
                        {
                            var newHeader = _mapper.Map<ResponseHeader>(headerDto);
                            entity.ResponseHeaders.Add(newHeader);
                        }
                    }
                

                await _repository.UpdateAsync(entity);
            }
        }

        //PATCH İŞLEMİ 
        public async Task PatchAsync(Guid id, UpdateResponseLogDto dto)
        {
            var existingLog = await _repository.GetByIdAsync(id);
            if (existingLog != null)
            {
                if (dto.StatusCode != 0) existingLog.StatusCode = dto.StatusCode;
                if (!string.IsNullOrEmpty(dto.Body)) existingLog.Body = dto.Body;
                if (dto.ExecutionTimeMs != 0) existingLog.ExecutionTimeMs = dto.ExecutionTimeMs;

              
                    var incomingHeaderIds = dto.ResponseHeaders
                        .Where(h => h.Id != Guid.Empty)
                        .Select(h => h.Id)
                        .ToList();

                    var headersToRemove = existingLog.ResponseHeaders.Where(h => !incomingHeaderIds.Contains(h.Id)).ToList();
                    foreach (var h in headersToRemove)
                    {
                        existingLog.ResponseHeaders.Remove(h);
                    }

                    foreach (var headerDto in dto.ResponseHeaders)
                    {
                        if (headerDto.Id != Guid.Empty)
                        {
                            var existingHeader = existingLog.ResponseHeaders.FirstOrDefault(h => h.Id == headerDto.Id);
                            if (existingHeader != null)
                            {
                                _mapper.Map(headerDto, existingHeader);
                            }
                        }
                        else
                        {
                            var newHeader = _mapper.Map<ResponseHeader>(headerDto);
                            existingLog.ResponseHeaders.Add(newHeader);
                        }
                    }
                

                await _repository.UpdateAsync(existingLog);
            }
        }
    }
}
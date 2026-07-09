using AutoMapper;
using SESMAN.Application.DTOs;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.Entities;
using SESMAN.Domain.Enums;
using SESMAN.Domain.ReposInterfaces;

namespace SESMAN.Application.Services
{
    public class RequestLogService : BaseService<RequestLog, RequestLogDto, CreateLogDto, UpdateLogDto>, IRequestLogService
    {
        public RequestLogService(IRequestLogRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        //  PATCH İŞLEMİ 
        public async Task PatchAsync(Guid id, UpdateLogDto dto)
        {
            var existingLog = await _repository.GetByIdAsync(id);
            if (existingLog != null)
            {
                // Ana Tablo Güncellemesi
                if (!string.IsNullOrEmpty(dto.Url)) existingLog.Url = dto.Url;
                if (!string.IsNullOrEmpty(dto.Method)) existingLog.Method = Enum.Parse<HttpMethodType>(dto.Method, true);
                if (!string.IsNullOrEmpty(dto.Body)) existingLog.Body = dto.Body;

                // BAŞLIKLAR İÇİN AKILLI GÜNCELLEME
                if (dto.RequestHeaders != null)
                {
                    var incomingHeaderIds = dto.RequestHeaders
                        .Where(h => h.Id != Guid.Empty)
                        .Select(h => h.Id)
                        .ToList();

                    var headersToRemove = existingLog.RequestHeaders.Where(h => !incomingHeaderIds.Contains(h.Id)).ToList();
                    foreach (var h in headersToRemove)
                    {
                        existingLog.RequestHeaders.Remove(h);
                    }

                    foreach (var headerDto in dto.RequestHeaders)
                    {
                        if (headerDto.Id != Guid.Empty)
                        {
                            var existingHeader = existingLog.RequestHeaders.FirstOrDefault(h => h.Id == headerDto.Id);
                            if (existingHeader != null)
                            {
                                _mapper.Map(headerDto, existingHeader);
                            }
                        }
                        else
                        {
                            var newHeader = _mapper.Map<RequestHeader>(headerDto);
                            existingLog.RequestHeaders.Add(newHeader);
                        }
                    }
                }

                
                if (dto.RequestParameters != null)
                {
                    var incomingParamIds = dto.RequestParameters
                        .Where(p => p.Id != Guid.Empty)
                        .Select(p => p.Id)
                        .ToList();

                    var paramsToRemove = existingLog.RequestParameters.Where(p => !incomingParamIds.Contains(p.Id)).ToList();
                    foreach (var p in paramsToRemove)
                    {
                        existingLog.RequestParameters.Remove(p);
                    }

                    foreach (var paramDto in dto.RequestParameters)
                    {
                        if (paramDto.Id != Guid.Empty)
                        {
                            var existingParam = existingLog.RequestParameters.FirstOrDefault(p => p.Id == paramDto.Id);
                            if (existingParam != null)
                            {
                                _mapper.Map(paramDto, existingParam);
                            }
                        }
                        else
                        {
                            var newParam = _mapper.Map<RequestParameter>(paramDto);
                            existingLog.RequestParameters.Add(newParam);
                        }
                    }
                }

               
                await _repository.UpdateAsync(existingLog);
            }
        }

        // PUT İŞLEMİ
        public override async Task UpdateAsync(Guid id, UpdateLogDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                // Ana Tablo Güncellemesi
                _mapper.Map(dto, entity);

                // BAŞLIKLAR İÇİN AKILLI GÜNCELLEME
                if (dto.RequestHeaders != null)
                {
                    var incomingHeaderIds = dto.RequestHeaders
                        .Where(h => h.Id != Guid.Empty)
                        .Select(h => h.Id)
                        .ToList();

                    var headersToRemove = entity.RequestHeaders.Where(h => !incomingHeaderIds.Contains(h.Id)).ToList();
                    foreach (var h in headersToRemove)
                    {
                        entity.RequestHeaders.Remove(h);
                    }

                    foreach (var headerDto in dto.RequestHeaders)
                    {
                        if (headerDto.Id != Guid.Empty)
                        {
                            var existingHeader = entity.RequestHeaders.FirstOrDefault(h => h.Id == headerDto.Id);
                            if (existingHeader != null)
                            {
                                _mapper.Map(headerDto, existingHeader);
                            }
                        }
                        else
                        {
                            var newHeader = _mapper.Map<RequestHeader>(headerDto);
                            entity.RequestHeaders.Add(newHeader);
                        }
                    }
                }

                // PARAMETRELER İÇİN AKILLI GÜNCELLEME
                if (dto.RequestParameters != null)
                {
                    var incomingParamIds = dto.RequestParameters
                        .Where(p => p.Id != Guid.Empty)
                        .Select(p => p.Id)
                        .ToList();

                    var paramsToRemove = entity.RequestParameters.Where(p => !incomingParamIds.Contains(p.Id)).ToList();
                    foreach (var p in paramsToRemove)
                    {
                        entity.RequestParameters.Remove(p);
                    }

                    foreach (var paramDto in dto.RequestParameters)
                    {
                        if (paramDto.Id != Guid.Empty)
                        {
                            var existingParam = entity.RequestParameters.FirstOrDefault(p => p.Id == paramDto.Id);
                            if (existingParam != null)
                            {
                                _mapper.Map(paramDto, existingParam);
                            }
                        }
                        else
                        {
                            var newParam = _mapper.Map<RequestParameter>(paramDto);
                            entity.RequestParameters.Add(newParam);
                        }
                    }
                }

                await _repository.UpdateAsync(entity);
            }
        }
    }
}
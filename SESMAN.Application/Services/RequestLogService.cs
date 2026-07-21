using AutoMapper;
using SESMAN.Application.DTOs;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.Entities;
using SESMAN.Domain.Enums;
using SESMAN.Domain.ReposInterfaces;

namespace SESMAN.Application.Services
{
    public class RequestLogService : BaseService<RequestLog, RequestLogDto, CreateRequestLogDto, UpdateRequestLogDto>, IRequestLogService
    {
        public RequestLogService(IRequestLogRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public new async Task<IEnumerable<RequestLogDto>> GetAllAsync()
        {
            // Application katmanı bu Interface'i zaten tanıdığı için hata vermiyor.
            var customRepo = (IRequestLogRepository)_repository;

            // Artık IRequestLogRepository içindeki Include'lu GetAllAsync çalışacak
            var logs = await customRepo.GetAllAsync();

            return _mapper.Map<IEnumerable<RequestLogDto>>(logs);
        }

        public async Task<IEnumerable<RequestLogDto>> GetPagedHistoryAsync(int page, int pageSize)
        {
            var customRepo = (IRequestLogRepository)_repository;

            // Artık _repository değil, customRepo üzerinden çağırıyoruz
            var logs = await customRepo.GetPagedHistoryAsync(page, pageSize);

            // Gelen ham veriyi DTO'ya çevirip Controller'a yolluyoruz
            return _mapper.Map<IEnumerable<RequestLogDto>>(logs);
        }

        //  PATCH İŞLEMİ 
        public async Task PatchAsync(Guid id, UpdateRequestLogDto dto)
        {
            var existingLog = await _repository.GetByIdAsync(id);
            if (existingLog != null)
            {
                // Ana Tablo Güncellemesi
                if (!string.IsNullOrEmpty(dto.Url)) existingLog.Url = dto.Url;
                if (!string.IsNullOrEmpty(dto.Method)) existingLog.Method = Enum.Parse<HttpMethodType>(dto.Method, true);
                if (!string.IsNullOrEmpty(dto.Body)) existingLog.Body = dto.Body;

                // headers güncelleme
              
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
                

               
                await _repository.UpdateAsync(existingLog);
            }
        }

        // PUT İŞLEMİ
        public override async Task<bool> UpdateAsync(Guid id, UpdateRequestLogDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }
                // Ana Tablo Güncellemesi
                _mapper.Map(dto, entity);

                // headers güncelleme
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

                // parametre güncelleme
             
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
                

                await _repository.UpdateAsync(entity);
            return true;
            }
        }
    }

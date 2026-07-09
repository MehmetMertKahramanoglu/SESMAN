using SESMAN.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.Interfaces
{
    public interface IResponseLogService : IBaseService<ResponseLogDto, CreateResponseLogDto, UpdateResponseLogDto>
    {
        Task PatchAsync(Guid id, UpdateResponseLogDto dto);
    }
}

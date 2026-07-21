using SESMAN.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.Interfaces
{
    public interface ISavedRequestService : IBaseService<SavedRequestDto, CreateSavedRequestDto, CreateSavedRequestDto>
    {
    }
}
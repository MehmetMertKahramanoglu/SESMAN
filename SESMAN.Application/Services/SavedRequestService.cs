using AutoMapper;
using SESMAN.Application.DTOs;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.Entities;
using SESMAN.Domain.ReposInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.Services
{
    public class SavedRequestService : BaseService<SavedRequest, SavedRequestDto, CreateSavedRequestDto, CreateSavedRequestDto>, ISavedRequestService
    {
       
        public SavedRequestService(ISavedRequestRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
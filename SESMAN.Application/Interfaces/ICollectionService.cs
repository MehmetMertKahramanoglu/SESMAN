using SESMAN.Application.DTOs;
using SESMAN.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.Interfaces
{
    public interface ICollectionService : IBaseService<CollectionDto, CreateCollectionDto, CreateCollectionDto>
    {
    }
}

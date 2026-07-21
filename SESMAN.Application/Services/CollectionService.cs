using AutoMapper;
using SESMAN.Application.DTOs;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.Entities;
using SESMAN.Domain.ReposInterfaces;

namespace SESMAN.Application.Services
{
    public class CollectionService : BaseService<Collection, CollectionDto, CreateCollectionDto, CreateCollectionDto>, ICollectionService
    {
        public CollectionService(ICollectionRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

    }
}
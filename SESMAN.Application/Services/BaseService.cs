using SESMAN.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.Services
{
    public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto> : IBaseService<TDto, TCreateDto, TUpdateDto> //veri dönüşümlerini yapan kısım
        where TEntity : class
    {
        protected readonly IBaseRepository<TEntity> _repository;

        protected BaseService(IBaseRepository<TEntity> repository)
        {
            _repository = repository;
        }

        // --- MANUEL MAPPING İÇİN sonradan kaldıracağım
        protected abstract TDto MapToDto(TEntity entity);
        protected abstract TEntity MapToEntity(TCreateDto createDto);
        protected abstract void MapUpdateDtoToEntity(TUpdateDto updateDto, TEntity entity);

        // CRUD işlemleri tek seferde 
        public async Task<List<TDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto).ToList(); 
        }

        public async Task<TDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? default : MapToDto(entity);
        }

        public async Task CreateAsync(TCreateDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(Guid id, TUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                MapUpdateDtoToEntity(dto, entity); 
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}

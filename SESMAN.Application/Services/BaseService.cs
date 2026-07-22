using AutoMapper;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.ReposInterfaces;

namespace SESMAN.Application.Services
{
    public class BaseService<TEntity, TDto, TCreateDto, TUpdateDto> : IBaseService<TDto, TCreateDto, TUpdateDto>
        where TEntity : class
    {
        protected readonly IBaseRepository<TEntity> _repository;
        protected readonly IMapper _mapper; 

        public BaseService(IBaseRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<TDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<List<TDto>>(entities); // AutoMapper otomatik listeyi çevirir
        }

        public async Task<TDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? default : _mapper.Map<TDto>(entity);
        }

        public async Task CreateAsync(TCreateDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _repository.AddAsync(entity); //burada otomatik olarak state ekleniyor. (EF tarafından)
        }

        public virtual async Task<bool> UpdateAsync(Guid id, TUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }
            _mapper.Map(dto, entity); // DTO'daki değişiklikleri mevcut entity üzerine yazar
            await _repository.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            //direkt repo kısmına delete işlemi gönderiliyor. Bulunamazsa false dönüşü alınıyor.
            return await _repository.DeleteAsync(id);
        }
    }
}
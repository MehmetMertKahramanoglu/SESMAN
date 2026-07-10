using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Application.Interfaces
{
    //Zararı yok ama fazla olmuş başlangıç için
    public interface IBaseService<TDto, TCreateDto, TUpdateDto>
    {
        Task<List<TDto>> GetAllAsync();
        Task<TDto?> GetByIdAsync(Guid id);
        Task CreateAsync(TCreateDto dto);
        Task UpdateAsync(Guid id, TUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}

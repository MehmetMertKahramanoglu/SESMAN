using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Domain.ReposInterfaces
{
    public interface IBaseRepository<T> where T : class //Bütün tablolar için geçerli olan kısımlar (hangi tablo gelirse T değeri o olur.)
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }
}

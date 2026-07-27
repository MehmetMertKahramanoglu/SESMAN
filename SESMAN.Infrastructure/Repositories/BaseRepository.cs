using Microsoft.EntityFrameworkCore;
using SESMAN.Application.Interfaces;
using SESMAN.Domain.ReposInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); // Hangi tablo gelirse onu ayarlar
        }

        public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync(); //Bütün değerleri döndürür.

        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id); //Tek değer döndürür.

        public async Task AddAsync(T entity) //değer ekleme
        {
            await _dbSet.AddAsync(entity); //burada ef verilere Added etiketi basar ve eklemeye hazır hale getirir.
            await _context.SaveChangesAsync(); //normalde bütün Added etiketli verileri db'ye kaydetme işine yarar ama biz override ettiğimiz için AppDbContext içindeki çalışıyor.
        }

        public virtual async Task UpdateAsync(T entity) //değer güncelleme
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id) //değer silme
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity == null)
            {
                return false; // Bulunamadıysa false dön
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true; // Başarıyla silindiyse true dön
        }
    }
    }

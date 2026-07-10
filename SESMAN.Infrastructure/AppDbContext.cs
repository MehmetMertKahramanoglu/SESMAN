using Microsoft.EntityFrameworkCore;
using SESMAN.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SESMAN.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RequestLog> RequestLogs { get; set; } 
        public DbSet<ResponseLog> ResponseLogs  { get; set; }
        public DbSet<RequestHeader> RequestHeaders  { get; set; }
        public DbSet<ResponseHeader> ResponseHeaders  { get; set; }
        public DbSet<RequestParameter> RequestParameters  { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestLog>()
                .HasOne(req => req.Response)
                .WithOne()
                .HasForeignKey<ResponseLog>(res => res.RequestLogId)
                .OnDelete(DeleteBehavior.Cascade); // İstek silinirse, cevabı da veritabanından silinsin

            modelBuilder.Entity<RequestLog>() //enum yapısında normalde int değer döneceği için okumayı kolaylaştırması için stringe döndürüyorum. (2 yerine post yazacak.)
                .Property(r => r.Method)
                .HasConversion<string>();

            modelBuilder.Entity<RequestLog>()
         .HasOne(req => req.Response)        
         .WithOne(res => res.RequestLog)     
         .HasForeignKey<ResponseLog>(res => res.RequestLogId);

            modelBuilder.Entity<ResponseLog>()
        .HasMany(r => r.ResponseHeaders)
        .WithOne() 
        .HasForeignKey(h => h.ResponseLogId)
        .OnDelete(DeleteBehavior.Cascade);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Tracker veritabanına gitmek üzere olan tüm verileri yakalar
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                // Eğer bu yeni eklenen (Insert) bir kayıt ise oluşturulma tarihini bas
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }

                // Güncelleme anında o anki tarihi UpdatedAt'e bas
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
        
    }


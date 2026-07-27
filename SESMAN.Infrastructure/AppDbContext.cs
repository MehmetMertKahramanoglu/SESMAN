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


        public DbSet<SavedRequest> SavedRequests { get; set; }
        public DbSet<SavedRequestHeader> SavedRequestHeaders { get; set; }
        public DbSet<SavedRequestParameter> SavedRequestParameters { get; set; }
        public DbSet<Collection> Collections { get; set; }


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


            modelBuilder.Entity<SavedRequest>()
     .HasMany(s => s.SavedRequestHeaders) // SavedRequest'in içinde birden çok Header vardır
     .WithOne()                           // AMA Header'ın içinde geriye dönüş (Navigation Property) YOKTUR! (İçi boş)
     .HasForeignKey(h => h.SavedRequestId)
     .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavedRequest>()
        .HasMany(s => s.SavedRequestParameters) // SavedRequest'in içinde birden çok Parameter vardır
        .WithOne()                              // AMA Parameter'ın içinde geriye dönüş YOKTUR!
        .HasForeignKey(p => p.SavedRequestId)
        .OnDelete(DeleteBehavior.Cascade);


        //JSON çevirme kısımları
            base.OnModelCreating(modelBuilder);
            //Request kısmı için Json'a çevirme kısmı
            modelBuilder.Entity<RequestLog>(builder =>
            {
            builder.OwnsOne(r => r.Auth, authBuilder=>
                {
                    authBuilder.ToJson();
            });
            });

            //saved request kısmı için Json'a çevirme kısmı
            modelBuilder.Entity<SavedRequest>(builder =>
            {
            builder.OwnsOne(r => r.Auth, authBuilder => 
                    {
                        authBuilder.ToJson();
            });
            });
        }

        //bu kısım tarih atamayı tek yerden yapmak için var. Her serviste tek tek tarih ataması yapmamak için.
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Tracker veritabanına gitmek üzere olan tüm verileri yakalar(BaseEntity soyundan gelenleri yakalar.)
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                // Eğer bu yeni eklenen (Insert) bir kayıt ise oluşturulma tarihini bas (BaseRepository içinde AddAsync ile Added etiketi basılmıştı üzerine.)
                if (entry.State == EntityState.Added) //yeni kayıt olduğunda buraya girer
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }

                // Güncelleme anında o anki tarihi UpdatedAt'e bas
                else if (entry.State == EntityState.Modified) //eğer güncellemeyse buraya girer
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken); //buradaki base ifadesiyle normalde EF içindeki SaveChangesAsync çalışır ve db ye yazılır.
        }
    }
        
    }


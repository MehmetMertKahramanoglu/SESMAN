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

        public DbSet<RequestLog> HistoryLogs { get; set; } //bunun adını HistoryLogs yapıp, application layerda sonradan birleştirme yapacağım.
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
        }
    }
        
    }


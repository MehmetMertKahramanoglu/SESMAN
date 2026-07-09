using Microsoft.EntityFrameworkCore;
using SESMAN.Application.Interfaces;
using SESMAN.Application.Services;
using SESMAN.Domain.ReposInterfaces;
using SESMAN.Infrastructure;
using SESMAN.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Uygulama (Application) Katmanındaki Şefimizi kaydediyoruz
builder.Services.AddScoped<IRequestLogService, RequestLogService>();

// Altyapı (Infrastructure) Katmanındaki İşçimizi kaydediyoruz
builder.Services.AddScoped<IRequestLogRepository, RequestLogRepository>();


builder.Services.AddScoped<IResponseLogRepository, ResponseLogRepository>();
builder.Services.AddScoped<IResponseLogService, ResponseLogService>();

//AutoMapper için
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<SESMAN.Application.Mappings.MappingProfile>()); builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//DB bağlantısı olması için yazdım
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();

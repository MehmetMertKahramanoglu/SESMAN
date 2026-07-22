using Microsoft.EntityFrameworkCore;
using SESMAN.Application.Interfaces;
using SESMAN.Application.Services;
using SESMAN.Domain.ReposInterfaces;
using SESMAN.Infrastructure;
using SESMAN.Infrastructure.Repositories;
using SESMAN.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // C#'ın Enum (sayısal) değerleri JSON'a çevirirken kelime (String) olarak çevirmesini sağlar
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
// Application katmanının kaydı
builder.Services.AddScoped<IRequestLogService, RequestLogService>();

// Infrastructure katmanının kaydı
builder.Services.AddScoped<IRequestLogRepository, RequestLogRepository>();

// REPOSITORY BAĞLANTILARI
builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();
builder.Services.AddScoped<ISavedRequestRepository, SavedRequestRepository>();

// SERVICE BAĞLANTILARI
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<ISavedRequestService, SavedRequestService>();


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

//http kısmı için
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRestRequestService, RestRequestService>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            policy.WithOrigins("https://localhost:50171") // Vue'nun çalıştığı adres
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});



var app = builder.Build();

app.UseCors("AllowVueApp"); 

app.UseAuthorization();
app.MapControllers();


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

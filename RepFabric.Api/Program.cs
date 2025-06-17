using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RepFabric.Api.BL.Database;
using RepFabric.Api.BL.Enums;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.BL.Services;
using RepFabric.Api.Extensions;
using RepFabric.Api.Models.Common;
using System.Reflection;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
    string xmlFIle = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFIle));

});
builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection("Storage"));
builder.Services.Configure<ExcelUploadSettings>(builder.Configuration.GetSection("ExcelUpload"));
builder.Services.Configure<YoxelSettings>(builder.Configuration.GetSection("Yoxel"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient();
builder.Services.AddTransient<IYoxelSyncService, YoxelSyncService>();

// Register AWS S3 client
builder.Services.AddAWSService<IAmazonS3>();

// Register RepFabricContext with PostgreSQL
builder.Services.AddDbContext<RepFabricContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the correct storage service based on configuration
builder.Services.AddConfiguredFileStorageService();

builder.Services.AddTransient<IExcelTemplateService, ExcelTemplateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// Custom exception handling middleware
app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

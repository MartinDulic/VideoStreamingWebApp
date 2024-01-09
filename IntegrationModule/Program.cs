using BLL.Services;
using DAL.Repository;
using AutoMapper;
using BL.Maper;
using Microsoft.EntityFrameworkCore;
using DAL.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);





builder.Services.AddScoped<RepositoryFactory>();
builder.Services.AddScoped<CountryService>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<VideoService>();
builder.Services.AddScoped<VideoTagService>();
builder.Services.AddScoped<UserService>();

var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
var environmentName = builder.Environment.EnvironmentName;


builder.Configuration
    .SetBasePath(currentDirectory)
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
    .AddEnvironmentVariables();


builder.Services.AddDbContext<RwaProjectDatabaseContext>(options =>
{
    options.UseSqlServer("name=ConnectionStrings:RWAConnStr");
});

var app = builder.Build();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using BLL.Services;
using DAL.Model;
using DAL.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddAutoMapper(typeof(BL.Maper.AutoMapperProfile));

builder.Services.AddScoped<RwaProjectDatabaseContext>();
builder.Services.AddScoped<RepositoryFactory>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<CountryService>();
builder.Services.AddScoped<VideoService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<VideoTagService>();
builder.Services.AddScoped<UserService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Genre}/{action=Index}/{id?}");

app.Run();

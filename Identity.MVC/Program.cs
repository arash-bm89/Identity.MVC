using System.IdentityModel.Tokens.Jwt;
using Identity.MVC.Data;
using Identity.MVC.Models;
using Identity.MVC.Repository;
using Identity.MVC.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDatabase"));
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
// 1:
 builder.Services.AddScoped<IIdentityRepository<string, JwtSecurityToken>, JWTBasedIdentityRepository>();
// 2:
//builder.Services.AddScoped<IIdentityRepository<string, User>, CookieBasedIdentityRepository>();
builder.Services.AddSingleton<ICacheRepository<User>, SessionCacheRepository>();
builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("CacheDatabase");
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

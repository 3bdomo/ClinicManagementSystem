using BLL.Extensions;
using ClinicSystem.DAL.Models;
using Common.Interfaces;
using DAL.Extensions;
using Microsoft.AspNetCore.Identity;
using Web.Middleware;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDalServices(connectionString);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit      = true;
    options.Password.RequiredLength    = 8;
    options.User.RequireUniqueEmail    = true;
})
.AddEntityFrameworkStores<DAL.Context.ClinicDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddBllServices();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

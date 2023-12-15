using IdentityServer4.Validation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OauthServer;
using OauthServer.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer("server=.;database=OauthDBUI;Trusted_Connection=true;TrustServerCertificate=True");
});

builder.Services.AddTransient<IResourceOwnerPasswordValidator, UserResourceOwnerPasswordValidator>();

//builder.Services.AddDataProtection()
//    .PersistKeysToFileSystem(new DirectoryInfo(@"c:\PATH TO COMMON KEY APPLE FOLDER"))
//    .SetApplicationName("SharedIdentityCookieApp");


services.AddIdentity<IdentityUser, IdentityRole>(o =>
{
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireDigit = false;
    o.Password.RequiredLength = 4;
    o.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

services.AddIdentityServer()
.AddAspNetIdentity<IdentityUser>()
.AddClientStore<ClientStore>()
.AddResourceStore<ResourceStore>()
.AddDeveloperSigningCredential();

//services.ConfigureApplicationCookie(o =>
//{
//    //o.Cookie.SameSite = SameSiteMode.None;
//    //o.Cookie.SecurePolicy = CookieSecurePolicy.None;
//    // o.Cookie.Name = "IdentityServer.OAuthCookie";
//    //o.LoginPath = "/Auth/Login";
//    //o.Cookie.SameSite = SameSiteMode.None;
//    //o.Cookie.Path = "/";

//    o.Cookie.SameSite = SameSiteMode.None;
//    o.Cookie.Name = "appleCookie";
//    o.Cookie.Path = "/";

//    //o.Cookie.Name = "orangeCookie";
//    //o.Cookie.Path = "/app2";

//    o.LoginPath = "/Auth/Login";
//});

services.AddCors(o =>
{
    o.AddPolicy("AllowAll", o =>
    {
        o.WithOrigins("http://localhost:3005", "http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod();

    });
});


services.AddHttpClient().AddHttpContextAccessor();
services.AddControllersWithViews();

var app = builder.Build();


DBMigrate.Migration(app);
//app.CreateUser();
app.UseCookiePolicy();
app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

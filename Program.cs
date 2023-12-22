using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
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

services.AddIdentityServer()
.AddClientStore<ClientStore>()
.AddResourceStore<ResourceStore>()
.AddDeveloperSigningCredential();

services.AddHttpClient().AddHttpContextAccessor();
services.AddControllersWithViews();

var app = builder.Build();

DBMigrate.Migration(app);

app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

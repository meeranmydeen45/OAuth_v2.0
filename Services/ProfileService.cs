using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace OauthServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly AuthDbContext authDbContext;
        public ProfileService(AuthDbContext _authDbContext)
        {
            authDbContext = _authDbContext;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await authDbContext.User.Where(x => x.Id == 0).FirstOrDefaultAsync();

            var claims = new List<Claim>
        {
            new Claim("Role", "Admin"),
            new Claim("UserName", "Meeran"),
            new Claim("City", "NewYork"),
        };

            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await authDbContext.User.Where(x => x.Id == 0).FirstOrDefaultAsync();
            //context.IsActive = (user != null) && user.IsActive;
            context.IsActive = true;
        }
    }
}

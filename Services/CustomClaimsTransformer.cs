using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace OauthServer.Services
{
    public class CustomClaimsTransformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = (ClaimsIdentity)principal.Identity;

            // Add custom claims based on your requirements
            identity.AddClaim(new Claim("custom_claim", "custom_value"));

            return Task.FromResult(principal);
        }
    }
}

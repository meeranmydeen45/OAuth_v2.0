using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OauthServer.Request;
using System.Net.Http;

namespace OauthServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly HttpContext? context;
        private readonly string oAuthServerURL;

        public HomeController(IHttpClientFactory httpClientFactory, IHttpContextAccessor accessor)
        {
            httpClient = httpClientFactory.CreateClient();
            context = accessor.HttpContext;
            oAuthServerURL = "https://localhost:4000/";
        }
        public async Task<IActionResult> Index()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> GetAccessTokenByClientCredentials([FromBody]LoginModel model)
        {
            var document = await httpClient.GetDiscoveryDocumentAsync(oAuthServerURL);
            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = $"{oAuthServerURL}connect/token",
                ClientId = model.ClientId,
                ClientSecret = model.ClientSecret,
                Scope = string.Join(" ", model.Scopes),
            });

            var responseObj = new
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
            };

            return Ok(responseObj);
        }


        [HttpPost]
        public async Task<IActionResult> GetAccessTokenByUser([FromBody] LoginModel model)
        {
            var document = await httpClient.GetDiscoveryDocumentAsync(oAuthServerURL);
            var tokenResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = $"{oAuthServerURL}connect/token",
                ClientId = model.ClientId,
                ClientSecret = model.ClientSecret,
                Scope = string.Join(" ", model.Scopes),
                UserName = model.UserName,
                Password = model.Password
            });

         
            var responseObj = new
            {
                AccessToken = tokenResponse?.AccessToken,
                RefreshToken = tokenResponse?.RefreshToken,
            };

            return Ok(responseObj);
        }

        [HttpPost]
        public async Task<IActionResult> GetAccessTokenByRefreshToken([FromBody] LoginModel model)
        {
            var tokenResponse = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = $"{oAuthServerURL}connect/token",
                ClientId = model.ClientId,
                ClientSecret = model.ClientSecret,
                RefreshToken = model.RefreshToken ?? "NA",
                GrantType = "refresh_token"
            });

            var responseObj = new
            {
                AccessToken = tokenResponse?.AccessToken,
                RefreshToken = tokenResponse?.RefreshToken,
            };

            return Ok(responseObj);
        }
    }
}

using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace OauthServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly HttpContext? context;

        public HomeController(IHttpClientFactory httpClientFactory, IHttpContextAccessor accessor)
        {
            httpClient = httpClientFactory.CreateClient();
            context = accessor.HttpContext;
        }
        public async Task<IActionResult> Index()
        {
            return View("Index");
        }

        public async Task<IActionResult> GetToken()
        {
            var document = await httpClient.GetDiscoveryDocumentAsync("https://localhost:4000/");
            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "https://localhost:4000/connect/token",
                ClientId = "ClientOne",
                ClientSecret = "clientSecret1",
                Scope = "api.one",
    
            });

            return Ok(tokenResponse.AccessToken);
        }


        public IActionResult Privacy()
        {
            return View("Privacy");
        }
    }
}

using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OauthServer.Models;
using System.Net.Http;
using System.Security.Claims;

namespace OauthServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signinManager;
        private readonly AuthDbContext context;

        public AuthController(UserManager<IdentityUser> _userManager, SignInManager<IdentityUser> _siginManager, AuthDbContext _dbContext)
        {
            userManager = _userManager;
            signinManager = _siginManager;
            context = _dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var externalProviders = await signinManager.GetExternalAuthenticationSchemesAsync();
            return View(new LoginViewModel { ReturnUrl = returnUrl, ExternalProviders = externalProviders });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            string returnUrl = model.ReturnUrl;
            int indexofClientId = returnUrl.IndexOf("client_id");

            string clientIdWithExtras = returnUrl.Substring(indexofClientId);
            string clientName = clientIdWithExtras.Split('&')[0].Trim().Split('=')[1].Trim();

            int? ClientId = context.Client.Where(x => x.ClientId == clientName).FirstOrDefault()?.Id;
            if (ClientId == null)
            {
                throw new Exception();
            }

            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                throw new Exception();
            }

            var isFound = context.UserClient.Where(x => x.UserId == user.Id && x.ClientId == ClientId).FirstOrDefault();
            if (isFound == null)
            {
                return View("Block",model);
            }

            var res = await signinManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if(res.Succeeded)
            {
                return Redirect(model.ReturnUrl);
            }
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            var redirectUri = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = signinManager.ConfigureExternalAuthenticationProperties(provider, redirectUri);

            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var info = await signinManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            
            var result = await signinManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }

            var username = info.Principal.FindFirst(ClaimTypes.Name.Replace(" ", "_")).Value;
            return View("ExternalRegister", new ExternalRegisterViewModel
            {
                Username = username,
                ReturnUrl = returnUrl
            });
        }


        [HttpPost]
        public async Task<IActionResult> ExternalRegister(ExternalRegisterViewModel model)
        {
            var info = await signinManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }

            var user = new IdentityUser(model.Username);
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return View(model);
            }

            result = await userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                return View(model);
            }

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
            {
                await userManager.AddClaimAsync(user,
                    info.Principal.FindFirst(ClaimTypes.GivenName));
            }

            if (info.Principal.HasClaim(c => c.Type == "urn:google:locale"))
            {
                await userManager.AddClaimAsync(user,
                    info.Principal.FindFirst("urn:google:locale"));
            }

            if (info.Principal.HasClaim(c => c.Type == "urn:google:picture"))
            {
                await userManager.AddClaimAsync(user,
                    info.Principal.FindFirst("urn:google:picture"));
            }

            var props = new AuthenticationProperties();
            props.StoreTokens(info.AuthenticationTokens);
            props.IsPersistent = true;

            await signinManager.SignInAsync(user, props);
            return Redirect(model.ReturnUrl);
        }
    }
}

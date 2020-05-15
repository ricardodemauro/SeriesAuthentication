using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SeriesAuthZAuhN.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Policy = "Admin")]
        public IActionResult SecretApi() => Ok("Secret Api");

        [AllowAnonymous]
        public IActionResult Index() => Ok("Hello from index");

        [Authorize]
        public IActionResult Claims()
        {
            return Ok(User.Claims.Select(x => new { Type = x.Type, Value = x.Value }));
        }

        public async Task<IActionResult> Authentication()
        {
            ClaimsIdentity identity = new ClaimsIdentity("CookieSeriesAuth");

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1234"));
            identity.AddClaim(new Claim(ClaimTypes.Email, "bob@rmauro.dev"));
            identity.AddClaim(new Claim(ClaimTypes.Webpage, "https://rmauro.dev"));

            identity.AddClaim(new Claim(ClaimTypes.Role, "SecretRole"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "Student"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "Teen"));

            ClaimsPrincipal principal = new ClaimsPrincipal(new[] { identity });

            await HttpContext.SignInAsync(principal);

            return Redirect("/home/index");
        }
    }
}

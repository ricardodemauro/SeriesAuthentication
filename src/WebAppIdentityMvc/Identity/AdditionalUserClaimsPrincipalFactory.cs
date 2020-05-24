using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAppIdentityMvc.Models;

namespace WebAppIdentityMvc.Identity
{
    public class AdditionalUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public AdditionalUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);

            var identity = (ClaimsIdentity)principal.Identity;

            var claims = new List<Claim>();
            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "AdminBlaster"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "UserBlaster"));
            }

            claims.Add(new Claim(ClaimTypes.GivenName, user.Name));

            identity.AddClaims(claims);
            return principal;
        }
    }
}

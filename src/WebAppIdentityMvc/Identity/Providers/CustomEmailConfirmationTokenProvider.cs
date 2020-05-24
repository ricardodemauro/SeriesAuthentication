using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppIdentityMvc.Identity.Providers
{
    public class CustomEmailConfirmationTokenProvider<TUser>
        : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<DataProtectionTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
        }

        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            return base.CanGenerateTwoFactorTokenAsync(manager, user);
        }

        public override Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            return base.GenerateAsync(purpose, manager, user);
        }

        public override Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            return base.ValidateAsync(purpose, token, manager, user);
        }
    }
}

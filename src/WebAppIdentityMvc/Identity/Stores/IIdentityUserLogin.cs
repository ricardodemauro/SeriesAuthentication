using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppIdentityMvc.Identity.Stores
{
    public interface IIdentityUserLogin
    {
        List<UserLoginInfo> UserLogins { get; set; }

        void AddUserLogin(UserLoginInfo userLogin);

        void RemoveUserLogin(string loginProvider, string providerKey);
    }
}

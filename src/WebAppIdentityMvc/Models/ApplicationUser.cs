using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppIdentityMvc.Identity.Stores;

namespace WebAppIdentityMvc.Models
{
    public class ApplicationUser : IdentityUser<ObjectId>, IIdentityUserRole, IIdentityUserLogin
    {
        [PersonalData]
        public string Name { get; set; }

        public bool IsAdmin { get; set; }

        public virtual List<string> Roles { get; set; }

        public virtual List<UserLoginInfo> UserLogins { get; set; }

        public ApplicationUser()
        {
            Roles = new List<string>();
            UserLogins = new List<UserLoginInfo>();
        }

        public virtual void AddRole(string role)
        {
            Roles.Add(role);
        }

        public virtual void RemoveRole(string role)
        {
            Roles.Remove(role);
        }

        public void AddUserLogin(UserLoginInfo userLogin)
        {
            UserLogins.Add(userLogin);
        }

        public void RemoveUserLogin(string loginProvider, string providerKey)
        {
            UserLogins.RemoveAll(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppIdentityMvc.Models
{
    public class ApplicationRole : IdentityRole<ObjectId>
    {
        public override ObjectId Id { get; set; }

        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName)
            : this()
        {
            Name = roleName;
        }
    }
}

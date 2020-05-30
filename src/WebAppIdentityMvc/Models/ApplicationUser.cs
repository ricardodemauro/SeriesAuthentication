using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace WebAppIdentityMvc.Models
{
    public class ApplicationUser : IdentityUser<ObjectId>, IUserRoles
    {
        public override ObjectId Id { get; set; }

        [PersonalData]
        public string Name { get; set; }

        public bool IsAdmin { get; set; }

        public virtual List<string> Roles { get; set; }

        public ApplicationUser()
        {
            Roles = new List<string>();
        }

        public virtual void AddRole(string role)
        {
            Roles.Add(role);
        }

        public virtual void RemoveRole(string role)
        {
            Roles.Remove(role);
        }
    }
}

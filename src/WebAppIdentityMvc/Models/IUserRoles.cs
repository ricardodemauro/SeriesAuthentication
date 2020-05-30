using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppIdentityMvc.Models
{
    public interface IUserRoles
    {
        [BsonIgnoreIfNull]
        List<string> Roles { get; set; }

        void AddRole(string role);

        void RemoveRole(string role);
    }
}

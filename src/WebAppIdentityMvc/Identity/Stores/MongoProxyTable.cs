using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppIdentityMvc.Identity.Stores
{
    public class MongoProxyTable
    {
        private readonly string _connectionString;

        public const string TABLE_USERS = "Users";
        public const string TABLE_ROLES = "Roles";

        public MongoProxyTable(IConfiguration configuration)
        {
            _connectionString = configuration["MongoDbConnectionString"];
        }

        public IMongoCollection<T> GetCollection<T>(string tableName)
        {
            MongoClient client = new MongoClient(_connectionString);
            var db = client.GetDatabase("IdentityDb");
            return db.GetCollection<T>(tableName);
        }

        public void CreateCollection(string tableName)
        {
            MongoClient client = new MongoClient(_connectionString);
            var db = client.GetDatabase("IdentityDb");
        }
    }
}

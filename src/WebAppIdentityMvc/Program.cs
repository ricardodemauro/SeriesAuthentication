using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Extensions.Hosting;
using WebAppIdentityMvc.Identity.Stores;
using System.Threading.Tasks;
using WebAppIdentityMvc.Models;

namespace WebAppIdentityMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            InitializeTableStorage(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static void InitializeTableStorage(IHost host)
        {
            var db = host.Services.GetService<MongoProxyTable>();

            db.CreateCollection(MongoProxyTable.TABLE_USERS);
            db.CreateCollection(MongoProxyTable.TABLE_ROLES);
        }
    }
}

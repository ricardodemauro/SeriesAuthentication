using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAppIdentityMvc.Email;

namespace WebAppIdentityMvc
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<SendGridEmailOptions>(opt =>
            {
                opt.SendGridKey = configuration["SendGrid:SendGridKey"];
                opt.SendGridUser = configuration["SendGrid:SendGridUser"];
                opt.SendGridSender = configuration["SendGrid:SendGridSender"];
            });

            services.AddTransient<IEmailSender, SendGridEmailSender>();

            return services;
        }
    }
}

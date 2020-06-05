using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using Octokit;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using WebAppIdentityMvc.Identity;
using WebAppIdentityMvc.Identity.Stores;
using WebAppIdentityMvc.Models;

namespace WebAppIdentityMvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddTransient<MongoTablesFactory>();

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("Administrator", policyBuilder => policyBuilder.RequireRole("Admin"));
            });

            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opts.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = "Github";
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
                {
                    opt.Cookie.Name = "IdentityCookieeeeeeeeeeeeeah";
                })
                .AddOAuth("GitHub", options =>
                {
                    options.ClientId = Configuration["GitHub:ClientId"];
                    options.ClientSecret = Configuration["GitHub:ClientSecret"];
                    options.CallbackPath = new PathString("/github-oauth");
                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";
                    options.SaveTokens = true;
                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    options.ClaimActions.MapJsonKey("urn:github:login", "login");
                    options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
                    options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");
                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("token", context.AccessToken);
                            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();
                            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                            context.RunClaimActions(json.RootElement);
                        },
                        OnRedirectToAuthorizationEndpoint = ctx =>
                        {
                            ctx.HttpContext.Response.Redirect(ctx.RedirectUri);
                            return Task.CompletedTask;
                        },
                        //OnAccessDenied = ctx =>
                        //{
                        //    return Task.CompletedTask;
                        //},
                        //OnRemoteFailure = ctx =>
                        //{
                        //    return Task.CompletedTask;
                        //},
                        //OnTicketReceived = ctx =>
                        //{
                        //    return Task.CompletedTask;
                        //}
                    };
                });

            services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;

                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);

                opt.SignIn.RequireConfirmedAccount = false;
            })
            .AddDefaultUI()
            .AddDefaultTokenProviders()
            .AddRoleStore<MongoRoleStore<ApplicationRole, ObjectId>>()
            .AddUserStore<MongoUserStore<ApplicationUser, ObjectId>>();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>,
                AdditionalUserClaimsPrincipalFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.Use(async (context, next) =>
            //{
            //    context.Response.ContentType = "text/plain";

            //    // Request method, scheme, and path
            //    await context.Response.WriteAsync($"Request Method: {context.Request.Method}{Environment.NewLine}");
            //    await context.Response.WriteAsync($"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
            //    await context.Response.WriteAsync($"Request Path: {context.Request.Path}{Environment.NewLine}");

            //    // Headers
            //    await context.Response.WriteAsync($"Request Headers:{Environment.NewLine}");

            //    foreach (var header in context.Request.Headers)
            //    {
            //        await context.Response.WriteAsync($"{header.Key}: " +
            //                                          $"{header.Value}{Environment.NewLine}");
            //    }

            //    await context.Response.WriteAsync(Environment.NewLine);

            //    // Connection: RemoteIp
            //    await context.Response.WriteAsync($"Request RemoteIp: {context.Connection.RemoteIpAddress}");
            //    await next();
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

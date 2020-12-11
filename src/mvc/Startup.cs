
using IdentityModel;

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Threading.Tasks;

namespace mvc
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
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("cookie", c =>
                {
                    c.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                    c.Cookie.Name = "af3dfb27-78d6-4cc5-859d-06bf5075c0e5";
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://localhost:44310";//ids4 url
                    options.ClientId = "oidcClient";
                    options.ClientSecret = "SuperSecretPassword";
                    options.ResponseType = "code";
                    options.UsePkce = true;
                    options.ResponseMode = "query";
                    // options.CallbackPath = "/signin-oidc"; // default redirect URI

                    // options.Scope.Add("oidc"); // default scope
                    // options.Scope.Add("profile"); // default scope
                    options.Scope.Add("weather.read");
                    options.Scope.Add(JwtClaimTypes.Email);

                    
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
                    options.SignInScheme = "cookie";
                    options.SignedOutRedirectUri = "/home/LoggedOut";
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRemoteFailure = context =>
                        {
                            context.Response.Redirect("/");
                            context.HandleResponse();
                            return Task.FromResult(0);
                        },
                        OnUserInformationReceived = (ctx =>
                        {
                            return Task.Run(() =>
                            {
                                foreach (var c in ctx.Principal.Claims)
                                {
                                    Console.WriteLine($"{c.Type}:{c.Value}");

                                }
                            });
                        })
                    };


                });

            services.AddControllersWithViews();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();


            });
        }
    }
}

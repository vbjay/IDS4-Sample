
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Linq;
using System.Reflection;

namespace ids4
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            const string connectionString =
                @"Data Source=(LocalDb)\MSSQLLocalDB;database=Test.IdentityServer4.EntityFramework;trusted_connection=yes;";
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(builder =>
            builder.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly)));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddCors(config =>
            {
                //for now we are making cors WIDE OPEN.  You would not do this in a production system.  
                //The client configuration can be used instead to say client x has x cors endpoints and let ids handle the cors policy
                config.AddDefaultPolicy(pb =>
                {
                    pb.AllowAnyOrigin();
                    pb.AllowAnyMethod();
                    pb.AllowAnyHeader();
                    pb.SetIsOriginAllowedToAllowWildcardSubdomains();
                });
            });
            services
                .AddIdentityServer()
                .AddOperationalStore(
                options => options.ConfigureDbContext =
                builder => builder.UseSqlServer(
                    connectionString,
                    sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly))
                )
                .AddConfigurationStore(
                options => options.ConfigureDbContext =
                builder => builder.UseSqlServer(
                    connectionString,
                    sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly))
                )
                .AddAspNetIdentity<IdentityUser>()
                .AddDeveloperSigningCredential();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                InitializeDbTestData(app);
            }

            app.UseCors();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

        }
        private static void InitializeDbTestData(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
                
                //START clear all data to allow start fresh each debug-- remove for production.
                //Allows for updates from repositories folder to be applied in db.  
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                context.Clients.RemoveRange(context.Clients);
                //context.SaveChanges();
                context.IdentityResources.RemoveRange(context.IdentityResources);
                //context.SaveChanges();
                context.ApiScopes.RemoveRange(context.ApiScopes);
                //context.SaveChanges();
                context.ApiResources.RemoveRange(context.ApiResources);
                context.SaveChanges();
                
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                if (userManager.Users.Any())
                {
                    foreach (var usr in userManager.Users.ToList()) {
                        userManager.DeleteAsync(usr).Wait();
                    }
                }
                //END clear all data to allow start fresh each debug

                if (!context.Clients.Any())
                {
                    foreach (var client in Clients.Get())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Resources.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var scope in Resources.GetApiScopes())
                    {
                        context.ApiScopes.Add(scope.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Resources.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

              
                if (!userManager.Users.Any())
                {
                    foreach (var testUser in Users.Get())
                    {
                        var identityUser = new IdentityUser(testUser.Username)
                        {
                            Id = testUser.SubjectId
                        };

                        userManager.CreateAsync(identityUser, testUser.Password).Wait();
                        userManager.AddClaimsAsync(identityUser, testUser.Claims.ToList()).Wait();
                    }
                }
            }
        }

    }
}

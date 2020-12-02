
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ids4
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

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
                .AddInMemoryClients(Clients.Get())
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiResources(Resources.GetApiResources())
                .AddInMemoryApiScopes(Resources.GetApiScopes())
                .AddTestUsers(Users.Get())
                .AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseRouting();
            app.UseIdentityServer();

            app
                .UseEndpoints(endpoints =>
                {
                    endpoints
                        .MapGet("/",
                        async context =>
                        {
                            await context.Response.WriteAsync("Hello World!");
                        });
                });
        }
    }
}

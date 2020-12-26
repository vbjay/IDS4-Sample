using api.Swashbuckle.Filters;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using System;
using System.Collections.Generic;

namespace api
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
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.ApiName = "weather";
                    options.Authority = Configuration["Auth:Authority"];// pay attention to the correct port the ids project runs under when you debug.
                });

            services.AddAuthorization(c =>
            {
                c.AddPolicy("weather.read", p =>
                {
                    p.RequireClaim("scope", "weather.read");
                });

                c.AddPolicy("weather.write", p =>
                {
                    p.RequireClaim("scope", "weather.write");
                });
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "api", Version = "v1" });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Name = "weather-swagger",// client id
                    In = ParameterLocation.Cookie, // where the token will go
                    Flows = new OpenApiOAuthFlows()
                    {
                        ClientCredentials = new OpenApiOAuthFlow()
                        {
                            Scopes = new Dictionary<string, string> {
                                { "weather.read", "Read Access to Weather API" } ,
                                { "weather.write", "Write Access to Weather API" } },// the roles you want to get
                            TokenUrl = new Uri($"{Configuration["Auth:Authority"]}/connect/token"),
                        }
                    }
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme() { Reference = new OpenApiReference() { Type= ReferenceType.SecurityScheme, Id="oauth2" } },
                        new [] { "weather" }//api resource name
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1");
                    c.OAuthClientId("weather-swagger");
                    c.OAuthClientSecret("SuperSecretPassword");
                });


                app.UseHttpsRedirection();

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
            
        }
    }
}

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using Play.Common.Settings;
using Play.Identity.Service.Entities;
using Play.Identity.Service.HostedServices;

namespace Play.Identity.Service
{
    public class Startup
    {
        private const string ALLOWED_ORIGIN_SETTINGS = "AllowedOrigin";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //MongoDB natively stores Guid values as Binary type by default, By registering GuidSerializer with BsonType.String, it tells MongoDB to serialize and deserialize Guid values as strings. T
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

            var serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            var identityServerSettings = Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();

            // Registers the default ASP.NET Core Identity services for managing users and roles in the application.
            services.Configure<IdentitySettings>(Configuration.GetSection(nameof(IdentitySettings)))
                .AddDefaultIdentity<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                //Integrates MongoDB as the persistence store for ASP.NET Core Identity user and role data.
                // Specifies the type "Guid" used for the identifier (ID) of users and roles.
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
                (
                    mongoDbSettings.ConnectionString,
                    serviceSettings.ServiceName
                );

            // Configures the IdentityServer middleware in your ASP.NET Core application.
            // IdentityServer is used for handling authentication and authorization, issuing tokens, and managing user claims.
            services.AddIdentityServer(options =>
            {
                // Configures IdentityServer to raise events for successful, failed, and erroneous authentication operations.
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
            })
            // Integrates IdentityServer with ASP.NET Core Identity.
            // Specifies that IdentityServer should use the ApplicationUser class for user management.
            // ApplicationUser typically represents your application's user entity and allows IdentityServer to authenticate users
            // using the same user store as ASP.NET Core Identity.
            .AddAspNetIdentity<ApplicationUser>()
            // API scopes define the permissions or resources that clients can request access to.
            // The scopes are defined in the appsettings.json or appsettings.Development.json configuration files.
            .AddInMemoryApiScopes(identityServerSettings.ApiScopes)
            // API resources represent the APIs that IdentityServer is protecting. Each API resource can have one or more scopes associated with it.
            // The API resources are defined in the appsettings.json configuration file.
            .AddInMemoryApiResources(identityServerSettings.ApiResources)
            // Clients are applications or services that request tokens from IdentityServer.
            // The client information is defined in the appsettings.Development.json configuration file, allowing different configurations for development environments.
            .AddInMemoryClients(identityServerSettings.Clients)
            // Identity resources represent the claims about the user that clients can request, such as user profile information.
            // The identity resources are defined in the appsettings.Development.json configuration file.
            .AddInMemoryIdentityResources(identityServerSettings.IdentityResources);

            services.AddLocalApiAuthentication();

            services.AddControllers();
            services.AddHostedService<IdentitySeedHostedService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Identity.Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Identity.Service v1"));
                app.UseCors(builder =>
                {
                    builder.WithOrigins(Configuration[ALLOWED_ORIGIN_SETTINGS])
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
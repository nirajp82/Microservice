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

namespace Play.Identity.Service
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
            //MongoDB natively stores Guid values as Binary type by default, By registering GuidSerializer with BsonType.String, it tells MongoDB to serialize and deserialize Guid values as strings. T
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

            var serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            var identityServerSettings = Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();

            // Registers the default ASP.NET Core Identity services for managing users and roles in the application.
            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                //Integrates MongoDB as the persistence store for ASP.NET Core Identity user and role data.
                // Specifies the type "Guid" used for the identifier (ID) of users and roles.
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
                (
                    mongoDbSettings.ConnectionString,
                    serviceSettings.ServiceName
                );

            //sets up the IdentityServer middleware in your ASP.NET Core application, which will handle authentication and authorization tasks.
            services.AddIdentityServer((options =>
            {
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
            }))
            //Integrates IdentityServer with ASP.NET Core Identity. It specifies that IdentityServer should use the ApplicationUser class for user management, which is typically your application's user entity.
            .AddAspNetIdentity<ApplicationUser>()
            // Configures IdentityServer to use in-memory storage for API scopes. ApiScopes define the permissions or resources that clients can request access to.
            .AddInMemoryApiScopes(identityServerSettings.ApiScopes)
            //Configures IdentityServer to use in-memory storage for client definitions. Clients are applications that request tokens from IdentityServer.
            .AddInMemoryClients(identityServerSettings.Clients)
            // Configures IdentityServer to use in-memory storage for identity resources. Identity resources define the claims about the user that can be requested by clients.
            .AddInMemoryIdentityResources(identityServerSettings.IdentityResources);

            services.AddControllers();
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

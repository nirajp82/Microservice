using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Play.Common.Identity;
using Play.Common.MongoDb;
using Play.Common.RabbitMQ;
using Play.Common.Settings;
using Play.Trading.Service.StateMachines;

namespace Play.Trading.Service
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
            services.AddMongo()
                    .AddJwtBearerAuthentication();
            AddMassTransit(services);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Trading.Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Trading.Service v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddMassTransit(IServiceCollection services)
        {
            services.AddMassTransit(configure =>
            {
                configure.ConfigureRabbitMq();
                configure.AddSagaStateMachine<PurchaseStateMachine, PurchaseState>()
                    .MongoDbRepository(r =>
                    {
                        var serviceSettings = Configuration.GetSection(nameof(ServiceSettings))
                                                           .Get<ServiceSettings>();
                        var mongoSettings = Configuration.GetSection(nameof(MongoDbSettings))
                                                           .Get<MongoDbSettings>();                                                           

                        r.Connection = mongoSettings.ConnectionString;
                        r.DatabaseName = serviceSettings.ServiceName;
                    });
            });

            services.AddMassTransitHostedService();
        }
    }
}

using Play.Catalog.Service.Entities;
using Play.Common.Settings;
using Play.Common.MongoDb;
using MassTransit;
using Play.Catalog.Service.Settings;
using MassTransit.Definition;

var builder = WebApplication.CreateBuilder(args);

// Configure ServiceSettings from appsettings.json, mapping configuration values to the ServiceSettings class.
var serviceSettingsSection = builder.Configuration.GetSection(nameof(ServiceSettings));
builder.Services.Configure<ServiceSettings>(serviceSettingsSection);
var serviceSettings = serviceSettingsSection.Get<ServiceSettings>();

// Register Mongo database and repository
builder.Services
    .AddMongo()
    .AddMongoRepo<Item>("items");

builder.Services.AddMassTransit(c =>
{
    c.UsingRabbitMq((context, configurator) =>
     {
         var rabbitMQSettings = builder.Configuration.GetSection(nameof(RabbitMQSettings))
                                .Get<RabbitMQSettings>();
         configurator.Host(rabbitMQSettings!.Host);
         var endpointNameFormatter = new KebabCaseEndpointNameFormatter(serviceSettings!.ServiceName, false);
         configurator.ConfigureEndpoints(context, endpointNameFormatter);
     });
});

//Register a background service with the .NET Core application. 
//This service was responsible for managing the lifecycle of the MassTransit message bus.
//It ensured the bus was started when the application started and stopped when the application stopped.
builder.Services.AddMassTransitHostedService();

// Add services to the container.
builder.Services.AddControllers(
    options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
    }
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
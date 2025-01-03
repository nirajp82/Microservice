using Microsoft.OpenApi.Models;
using Play.Inventory.Service.Entities;
using Play.Common.MongoDb;
using Play.Common.Settings;
using Play.Common.RabbitMQ;
using Play.Common.Identity;
using GreenPipes.Configurators;
using GreenPipes;
using Play.Inventory.Service.Exceptions;

const string ALLOWED_ORIGIN_SETTING = "AllowedOrigin";
var builder = WebApplication.CreateBuilder(args);

// Configure ServiceSettings from appsettings.json, mapping configuration values to the ServiceSettings class.
var serviceSettingsSection = builder.Configuration.GetSection(nameof(ServiceSettings));
builder.Services.Configure<ServiceSettings>(serviceSettingsSection);

// Register Mongo database and repository
builder.Services
    .AddMongo()
    .AddMongoRepo<InventoryItem>("inventoryItems")
    .AddMongoRepo<CatalogItem>("catalogItems")
    .AddMassTransitWithRabbitMq(retryConfigurator => 
    {
        retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
        //No need to retry if UnknownItemException is thrown. Retry is for Transit errors only.
        retryConfigurator.Ignore(typeof(UnknownItemException));
    })
    .AddJwtBearerAuthentication();

var randomJitter = new Random();

/*Add Catalog Client

//Create Typed clients, It will add the IHttpClientFactory and related services to the IServiceCollection.
//https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory
builder.Services.AddHttpClient<CatalogClient>(cc =>
{
    cc.BaseAddress = new Uri("https://localhost:5001");
})
//Handle transient HTTP errors and timeouts gracefully. 
//It sets up a retry policy to automatically retry failed requests up to 5 times with 
//Exponentially increasing wait times between retries and adds a timeout policy 
//to cancel requests that exceed 1 second without receiving a response.
.AddPolicyHandler((serviceProvider, request) =>
    HttpPolicyExtensions.HandleTransientHttpError()
        .Or<TimeoutRejectedException>()
        .WaitAndRetryAsync
        (
            retryCount: 5,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                    + TimeSpan.FromMilliseconds(randomJitter.Next(0, 1000)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                var msg = $"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}";
                serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning("{msg}", msg);
            }
        ))
.AddPolicyHandler((serviceProvider, request) =>
    HttpPolicyExtensions.HandleTransientHttpError()
        .Or<TimeoutRejectedException>()
        .CircuitBreakerAsync
        (
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(15),
            onBreak: (outcome, timespan) =>
            {
                var msg = $"Opening the circuit for {timespan.TotalSeconds} seconds...";
                serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning("{msg}", msg);
            },
            onReset: () =>
            {
                var msg = $"Closing the circuit...";
                serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning("{msg}", msg);
            }
        ))
//Timeout the request if not completed with in 1 seconds
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
*/
// Add services to the container.
builder.Services.AddControllers(
    options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
    }
);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Inventory.Service", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(appBuilder =>
    {
        appBuilder.WithOrigins(builder.Configuration[ALLOWED_ORIGIN_SETTING]!)
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
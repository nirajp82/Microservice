using Play.Catalog.Service.Entities;
using Play.Common.Settings;
using Play.Common.MongoDb;
using Play.Common.RabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;

const string ALLOWED_ORIGIN_SETTING = "AllowedOrigin";
var builder = WebApplication.CreateBuilder(args);

// Configure ServiceSettings from appsettings.json, mapping configuration values to the ServiceSettings class.
var serviceSettingsSection = builder.Configuration.GetSection(nameof(ServiceSettings));
var serviceSettings =  serviceSettingsSection.Get<ServiceSettings>();
builder.Services.Configure<ServiceSettings>(serviceSettingsSection);

// Register Mongo database and repository
builder.Services
    .AddMongo()
    .AddMongoRepo<Item>("items")
    .AddMassTransitWithRabbitMq();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.Authority = "https://localhost:5003";
        options.Audience = serviceSettings?.ServiceName;
    });

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
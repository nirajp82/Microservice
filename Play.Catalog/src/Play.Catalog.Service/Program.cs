using Play.Catalog.Service.Entities;
using Play.Common.Settings;
using Play.Common.MongoDb;
using Play.Common.RabbitMQ;

const string ALLOWED_ORIGIN_SETTING = "AllowedOrigin";
var builder = WebApplication.CreateBuilder(args);

// Configure ServiceSettings from appsettings.json, mapping configuration values to the ServiceSettings class.
var serviceSettingsSection = builder.Configuration.GetSection(nameof(ServiceSettings));
builder.Services.Configure<ServiceSettings>(serviceSettingsSection);

// Register Mongo database and repository
builder.Services
    .AddMongo()
    .AddMongoRepo<Item>("items")
    .AddMassTransitWithRabbitMq();

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
        appBuilder.WithOrigins(builder.Configuration[ALLOWED_ORIGIN_SETTING])
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
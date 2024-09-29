using Play.Catalog.Service.Entities;
using Play.Common.Settings;
using Play.Common.MongoDb;
using Play.Common.RabbitMQ;
using Play.Common.Identity;
using Play.Catalog.Service;

const string ALLOWED_ORIGIN_SETTING = "AllowedOrigin";
var builder = WebApplication.CreateBuilder(args);

// Configure ServiceSettings from appsettings.json, mapping configuration values to the ServiceSettings class.
var serviceSettingsSection = builder.Configuration.GetSection(nameof(ServiceSettings));
var serviceSettings = serviceSettingsSection.Get<ServiceSettings>();
builder.Services.Configure<ServiceSettings>(serviceSettingsSection);

// Register Mongo database and repository
builder.Services
    .AddMongo()
    .AddMongoRepo<Item>("items")
    .AddMassTransitWithRabbitMq()
    .AddJwtBearerAuthentication();

// Configure authorization policies for the application
builder.Services.AddAuthorization(options =>
{
    // Define the 'Read' policy
    options.AddPolicy(Policies.Read, policy =>
    {
        // Require the user to have the 'Admin' role
        policy.RequireRole("Admin");

        // Require the user to have one of the specified claims for reading access
        policy.RequireClaim("scope", "catalog.readaccess", "catalog.fullaccess");
    });

    // Define the 'Write' policy
    options.AddPolicy(Policies.Write, policy =>
    {
        // Require the user to have the 'Admin' role
        policy.RequireRole("Admin");

        // Require the user to have one of the specified claims for writing access
        policy.RequireClaim("scope", "catalog.writeaccess", "catalog.fullaccess");
    });
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
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Reposiroty;
using Play.Catalog.Service.Settings;

var builder = WebApplication.CreateBuilder(args);

//In MongoDB, Guids can be represented as either Binary Data (BSON Binary type) or as a string. 
//By default, MongoDB's C# driver serializes Guids as Binary Data.
//So change the default settings to store them as strings for readability or compatibility reasons.
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
//Specifies that the DateTimeOffset objects should be serialized as strings (Bson.BsonType.String).
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

// Configure ServiceSettings from appsettings.json, mapping configuration values to the ServiceSettings class.
var serviceSettingsSection = builder.Configuration.GetSection(nameof(ServiceSettings));
var serviceSettings = serviceSettingsSection.Get<ServiceSettings>();
builder.Services.Configure<ServiceSettings>(serviceSettingsSection);

// AddMongoDB as singleton as it is threadsafe. MongoClient is thread-safe and serves as the root object for connecting to MongoDB servers and performing operations.
// https://mongodb.github.io/mongo-csharp-driver/2.0/reference/driver/connecting/#mongo-client
builder.Services.AddSingleton(serviceProvider =>
{
    // Retrieve MongoDB settings from appsettings.json
    MongoDbSettings mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>()!;

    // Create a MongoClient instance using the connection string from settings
    MongoClient mongoDbClient = new(connectionString: mongoDbSettings!.ConnectionString);

    // Access the service name from ServiceSettings to determine the database to connect to
    IMongoDatabase mongoDatabase = mongoDbClient.GetDatabase(serviceSettings!.ServiceName);

    // Register MongoDB database instance as a singleton service
    return mongoDatabase;
});

// Register ItemsRepository as a singleton service
builder.Services.AddSingleton<IItemsRepository, ItemsRepository>();

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
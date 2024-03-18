using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDb;

public static class Extensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        //In MongoDB, Guids can be represented as either Binary Data (BSON Binary type) or as a string. 
        //By default, MongoDB's C# driver serializes Guids as Binary Data. So change it to store them as strings
        BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));

        //Specifies that the DateTimeOffset objects should be serialized as strings (Bson.BsonType.String).
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

        // Add MongoDB as singleton object as it is threadsafe. 
        // https://mongodb.github.io/mongo-csharp-driver/2.0/reference/driver/connecting/#mongo-client
        services.AddSingleton(serviceProvider =>
        {
            IConfiguration configuration = serviceProvider.GetService<IConfiguration>()!;
            MongoDbSettings mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>()!;
            MongoClient mongoDbClient = new(connectionString: mongoDbSettings!.ConnectionString);

            //Retrive Service Settings Options that is configured as service in Program.cs
            IOptions<ServiceSettings> serviceSettingsOptions = serviceProvider.GetService<IOptions<ServiceSettings>>()!;
            //var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            //IMongoDatabase mongoDatabase = mongoDbClient.GetDatabase(serviceSettings!.ServiceName);

            // Access the service name from ServiceSettings to determine the database to connect to
            IMongoDatabase mongoDatabase = mongoDbClient.GetDatabase(serviceSettingsOptions.Value!.ServiceName);

            // Register MongoDB database instance as a singleton service
            return mongoDatabase;
        });

        return services;
    }

    public static IServiceCollection AddMongoRepo<T>(this IServiceCollection services, string collectionName) where T : IEntity
    {
        services.AddSingleton<IRepository<T>>(serviceProvider =>
        {
            IMongoDatabase mongoDatabase = serviceProvider.GetService<IMongoDatabase>()!;
            var mongoRepository = new MongoRepository<T>(mongoDatabase, collectionName);
            return mongoRepository;
        });

        return services;
    }
}
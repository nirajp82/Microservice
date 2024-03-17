namespace Play.Catalog.Service.Settings;

public class MongoDbSettings
{
    public string Host { get; init; } = null!;
    public int Port { get; init; }

    public string ConnectionString => $"mongodb://{Host}:{Port}";
}
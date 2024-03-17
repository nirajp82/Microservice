namespace Play.Common.Settings;

public class MongoDbSettings
{
    public string Host { get; init; } = null!;
    public string ConnectionString => $"mongodb://{Host}:{Port}";
    public int Port { get; init; }
}
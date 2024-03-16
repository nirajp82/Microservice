using MongoDB.Driver;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Reposiroty;

public class ItemsRepository : IItemsRepository
{
    private const string _dbName = "Catalog";
    private const string _collectionName = "items";
    private readonly IMongoCollection<Item> _dbCollection;
    private readonly FilterDefinitionBuilder<Item> _filterBuilder = Builders<Item>.Filter;

    public ItemsRepository(IMongoDatabase database)
    {
        //var mongoClient = new MongoClient("mongodb://localhost:27017");
        //var database = mongoClient.GetDatabase(_dbName);
        _dbCollection = database.GetCollection<Item>(_collectionName);
    }

    public async Task<IReadOnlyCollection<Item>> GetAllAsync()
    {
        var items = await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();
        return items;
    }

    public async Task<Item> GetAsync(Guid id)
    {
        FilterDefinition<Item> filter = _filterBuilder.Eq(entity => entity.Id, id);
        var item = await _dbCollection.Find(filter).FirstOrDefaultAsync();
        return item;
    }

    public async Task CreateAsync(Item entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(Item entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        FilterDefinition<Item> filter = _filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
        await _dbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        FilterDefinition<Item> filter = _filterBuilder.Eq(entity => entity.Id, id);
        await _dbCollection.DeleteOneAsync(filter);
    }
}
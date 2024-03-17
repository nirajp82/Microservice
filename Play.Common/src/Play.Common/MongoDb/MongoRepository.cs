using System.Linq.Expressions;
using MongoDB.Driver;

namespace Play.Common.MongoDb;

public class MongoRepository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T> _dbCollection;
    private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        //var mongoClient = new MongoClient("mongodb://localhost:27017");
        //var database = mongoClient.GetDatabase(_dbName);
        _dbCollection = database.GetCollection<T>(collectionName);
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        var items = await _dbCollection.Find(_filterBuilder.Empty).ToListAsync();
        return items;
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        var items = await _dbCollection.Find(filter).ToListAsync();
        return items;
    }

    public async Task<T> GetAsync(Guid id)
    {
        FilterDefinition<T> filter = _filterBuilder.Eq(entity => entity.Id, id);
        var dbEntity = await _dbCollection.Find(filter).FirstOrDefaultAsync();
        return dbEntity;
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
    {
        var dbEntity = await _dbCollection.Find(filter).FirstOrDefaultAsync();
        return dbEntity;
    }

    public async Task CreateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        FilterDefinition<T> filter = _filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
        await _dbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        FilterDefinition<T> filter = _filterBuilder.Eq(entity => entity.Id, id);
        await _dbCollection.DeleteOneAsync(filter);
    }
}
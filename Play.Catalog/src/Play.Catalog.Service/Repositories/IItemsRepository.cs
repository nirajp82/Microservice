using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Reposiroty;

public interface IItemsRepository
{
    Task CreateAsync(Item entity);
    Task<IReadOnlyCollection<Item>> GetAllAsync();
    Task<Item> GetAsync(Guid id);
    Task RemoveAsync(Guid id);
    Task UpdateAsync(Item entity);
}

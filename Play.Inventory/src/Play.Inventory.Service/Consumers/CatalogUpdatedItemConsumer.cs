using Play.Catalog.Contracts;
using MassTransit;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemUpdatedConsumer : IConsumer<CatalogItemUpdated>
{
    private readonly IRepository<CatalogItem> _repository;

    public CatalogItemUpdatedConsumer(IRepository<CatalogItem> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
    {
        CatalogItemUpdated catalogItem = context.Message;
        var dbItem = await _repository.GetAsync(catalogItem.ItemId);
        if (dbItem == null)
        {
            CatalogItem entity = new CatalogItem
            {
                Id = dbItem!.Id,
                Description = dbItem.Description,
                Name = dbItem.Name
            };
            await _repository.CreateAsync(entity);
        }
        else
        {
            dbItem.Description = catalogItem.Description;
            dbItem.Name = catalogItem.Name;
            await _repository.UpdateAsync(dbItem);
        }
    }
}
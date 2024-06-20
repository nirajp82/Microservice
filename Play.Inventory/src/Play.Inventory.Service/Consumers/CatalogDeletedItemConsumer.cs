using Play.Catalog.Contracts;
using MassTransit;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers;

public class CatalogDeletedItemConsumer : IConsumer<CatalogItemDeleted>
{
    private readonly IRepository<CatalogItem> _repository;

    public CatalogDeletedItemConsumer(IRepository<CatalogItem> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
    {
        CatalogItemDeleted catalogItem = context.Message;
        var dbItem = await _repository.GetAsync(catalogItem.ItemId);
        if (dbItem == null)
        {
            return;
        }
        await _repository.RemoveAsync(catalogItem.ItemId);
    }
}
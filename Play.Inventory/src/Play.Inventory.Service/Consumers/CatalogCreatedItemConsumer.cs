using Play.Catalog.Contracts;
using MassTransit;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
{
    private readonly IRepository<CatalogItem> _repository;

    public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<CatalogItemCreated> context)
    {
        CatalogItemCreated catalogItem = context.Message;
        var dbItem = await _repository.GetAsync(catalogItem.ItemId);
        if (dbItem != null)
        {
            return;
        }
        CatalogItem entity = new CatalogItem
        {
            Id = catalogItem.ItemId,
            Description = catalogItem.Description,
            Name = catalogItem.Name
        };
        await _repository.CreateAsync(entity);
    }
}
using MassTransit;
using Play.Common;
using Play.Inventory.Contracts;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Exceptions;
using System.Linq.Expressions;

namespace Play.Inventory.Service.Consumers;

public class GrantItemsConsumer : IConsumer<GrantItems>
{
    public readonly IRepository<CatalogItem> _catalogItemRepo;
    public readonly IRepository<InventoryItem> _inventoryItemRepo;

    public GrantItemsConsumer(IRepository<CatalogItem> catalogItemRepo, IRepository<InventoryItem> inventoryItemRepo)
    {
        _catalogItemRepo = catalogItemRepo;
        _inventoryItemRepo = inventoryItemRepo;
    }

    public async Task Consume(ConsumeContext<GrantItems> context)
    {
        var message = context.Message;
        var item = await _catalogItemRepo.GetAsync(message.CatalogItemId);

        if (item == null)
        {
            throw new UnknownItemException(message.CatalogItemId);
        }

        Expression<Func<InventoryItem, bool>> filter = item => item.UserId == message.UserId &&
                                                                item.CatalogItemId == message.CatalogItemId;
        var inventoryItem = await _inventoryItemRepo.GetAsync(filter);
        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem
            {
                CatalogItemId = message.CatalogItemId,
                UserId = message.UserId,
                Quantity = message.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };

            await _inventoryItemRepo.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += message.Quantity;
            await _inventoryItemRepo.UpdateAsync(inventoryItem);
        }

        await context.Publish(new InventoryItemsGranted(message.CorrelationId));
    }
}

using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    public readonly IRepository<InventoryItem> _itemsRepository;

    public ItemsController(IRepository<InventoryItem> itemsRepository)
    {
        _itemsRepository = itemsRepository;
    }

    [HttpGet]
    [ProducesResponseType<IEnumerable<InventoryItemDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return Results.BadRequest();
        }

        var items = (await _itemsRepository.GetAllAsync(item => item.UserId == userId))
                    .Select(item => item.AsDto());

        return Results.Ok(items);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IResult> PostAsync(GrantItemsDto grantItemsDto)
    {
        Expression<Func<InventoryItem, bool>> filter = item => item.UserId == grantItemsDto.UserId &&
                                                                item.CatalogItemId == grantItemsDto.CatalogItemId;
        var inventoryItem = await _itemsRepository.GetAsync(filter);
        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem
            {
                CatalogItemId = grantItemsDto.CatalogItemId,
                UserId = grantItemsDto.UserId,
                Quantity = grantItemsDto.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };

            await _itemsRepository.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grantItemsDto.Quantity;
            await _itemsRepository.UpdateAsync(inventoryItem);
        }

        return Results.Ok();
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private const string AdminRole = "Admin";

    //The IHttpClientFactory will go ahead and create the typed client ("CatalogClient") as needed. 
    //public readonly CatalogClient _catalogClient;
    public readonly IRepository<CatalogItem> _catalogItemRepo;
    public readonly IRepository<InventoryItem> _inventoryItemRepo;

    public ItemsController(IRepository<CatalogItem> catalogItemRepo, IRepository<InventoryItem> inventoryItemRepo)
    {
        _catalogItemRepo = catalogItemRepo;
        _inventoryItemRepo = inventoryItemRepo;
    }

    [HttpGet]
    [ProducesResponseType<IEnumerable<InventoryItemDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize]
    public async Task<IResult> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return Results.BadRequest();
        }

        var currentUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
        if (Guid.Parse(currentUserId) != userId) 
        {
            if(!User.IsInRole(AdminRole)) 
            {
                return Results.Forbid();
            }
        }

        var inventoryItems = await _inventoryItemRepo.GetAllAsync(item => item.UserId == userId);
        var catalogItemIds = inventoryItems.Select(i => i.CatalogItemId);
        var catalogItems = await _catalogItemRepo.GetAllAsync(item => catalogItemIds.Contains(item.Id));

        var inventoryItemDtos = inventoryItems.Select(item =>
        {
            var catalogItem = catalogItems?.FirstOrDefault(ci => ci.Id == item.CatalogItemId);
            return item.AsDto(catalogItem?.Name, catalogItem?.Description);
        });

        return Results.Ok(inventoryItemDtos);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = AdminRole)]
    public async Task<IResult> PostAsync(GrantItemsDto grantItemsDto)
    {
        Expression<Func<InventoryItem, bool>> filter = item => item.UserId == grantItemsDto.UserId &&
                                                                item.CatalogItemId == grantItemsDto.CatalogItemId;
        var inventoryItem = await _inventoryItemRepo.GetAsync(filter);
        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem
            {
                CatalogItemId = grantItemsDto.CatalogItemId,
                UserId = grantItemsDto.UserId,
                Quantity = grantItemsDto.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };

            await _inventoryItemRepo.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grantItemsDto.Quantity;
            await _inventoryItemRepo.UpdateAsync(inventoryItem);
        }

        return Results.Ok();
    }
}
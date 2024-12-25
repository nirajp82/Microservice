using System.Net;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Contracts;
using Play.Common;
using Microsoft.AspNetCore.Authorization;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private const string AdminRole = "Admin";

    private readonly IRepository<Item> _itemsRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    static int _requestCnt = 0;

    public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
    {
        _itemsRepository = itemsRepository;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    [ProducesResponseType<IEnumerable<ItemDto>>(StatusCodes.Status200OK)]
    [Authorize(Policies.Read)]
    public async Task<IResult> GetAsync()
    {
        //bool isPollyTest = false;
        //if (isPollyTest)
        //    await ClientPollyTest();

        var result = (await _itemsRepository.GetAllAsync())
                        .Select(Item => Item.AsDto());
        return Results.Ok(result);
    }

    // GET /items/{id}
    [HttpGet("{id}")]
    [ProducesResponseType<ItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policies.Read)]
    public async Task<IResult> GetByIdAsync(Guid id)
    {
        var item = await _itemsRepository.GetAsync(id);
        if (item == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(item.AsDto());
    }

    [HttpPost]
    [ProducesResponseType<ItemDto>(StatusCodes.Status201Created)]
    [Authorize(Policies.Write)]
    public async Task<IActionResult> PostAsync(CreateItemDto createItemDto)
    {
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = createItemDto.Name,
            Description = createItemDto.Description,
            Price = createItemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };
        await _itemsRepository.CreateAsync(item);
        await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description, item.Price));
        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, createItemDto);
    }

    // PUT /items/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policies.Write)]
    public async Task<IResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
    {
        var existingItem = await _itemsRepository.GetAsync(id);
        if (existingItem == null)
        {
            return Results.NotFound();
        }

        existingItem.Name = updateItemDto.Name;
        existingItem.Description = updateItemDto.Description;
        existingItem.Price = updateItemDto.Price;

        await _itemsRepository.UpdateAsync(existingItem);

        await _publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description, existingItem.Price));

        return Results.NoContent();
    }

    // DELETE /items/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policies.Write)]
    public async Task<IResult> Delete(Guid id)
    {
        var existingItem = await _itemsRepository.GetAsync(id);
        if (existingItem == null)
        {
            return Results.NotFound();
        }

        await _itemsRepository.RemoveAsync(id);
        await _publishEndpoint.Publish(new CatalogItemDeleted(id));
        return Results.NoContent();
    }

    async Task ClientPollyTest()
    {
        _requestCnt++;
        Console.WriteLine($"Request starting... Counter:{_requestCnt}");
        if (_requestCnt <= 2)
        {
            Console.WriteLine($"Delaying Request... Counter:{_requestCnt}");
            await Task.Delay(TimeSpan.FromSeconds(15));
        }
        if (_requestCnt <= 4)
        {
            Console.WriteLine($"Throw exception (500: Internal Server Error)... Counter:{_requestCnt} ");
            throw new BadHttpRequestException($"Throw Bad request exception... Counter:{_requestCnt}", (int)HttpStatusCode.BadRequest);
        }
    }
}
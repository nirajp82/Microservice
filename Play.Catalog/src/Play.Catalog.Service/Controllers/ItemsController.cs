using System.Net;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    public readonly IRepository<Item> _itemsRepository;
    static int _requestCnt = 0;

    public ItemsController(IRepository<Item> itemsRepository)
    {
        _itemsRepository = itemsRepository;
    }

    [HttpGet]
    [ProducesResponseType<IEnumerable<ItemDto>>(StatusCodes.Status200OK)]
    public async Task<IResult> GetAsync()
    {
        if (false)
        {
            await ClientPollyTest();
        }
        var result = (await _itemsRepository.GetAllAsync())
                        .Select(Item => Item.AsDto());
        return Results.Ok(result);
    }

    async Task ClientPollyTest()
    {
        _requestCnt++;
        Console.WriteLine($"Request starting... Counter:{_requestCnt}");
        if (_requestCnt <= 3)
        {
            Console.WriteLine($"Delaing Request... Counter:{_requestCnt}");
            await Task.Delay(1500);
        }
        if (_requestCnt < 15)
        {
            Console.WriteLine($"Throw exception... Counter:{_requestCnt}");
            throw new BadHttpRequestException($"Throw Bad request exception... Counter:{_requestCnt}", (int)HttpStatusCode.BadRequest);
        }
    }

    // GET /items/{id}
    [HttpGet("{id}")]
    [ProducesResponseType<ItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, createItemDto);
    }

    // PUT /items/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        return Results.NoContent();
    }

    // DELETE /items/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Delete(Guid id)
    {
        var existingItem = await _itemsRepository.GetAsync(id);
        if (existingItem == null)
        {
            return Results.NotFound();
        }

        await _itemsRepository.RemoveAsync(id);
        return Results.NoContent();
    }
}
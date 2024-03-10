using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private static readonly List<ItemDto> items = new()
    {
        new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
    };

    [HttpGet]
    [ProducesResponseType<IEnumerable<ItemDto>>(StatusCodes.Status200OK)]
    public IResult Get()
    {
        return Results.Ok(items);
    }

    // GET /items/{id}
    [HttpGet("{id}")]
    [ProducesResponseType<ItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IResult GetById(Guid id)
    {
        var item = items.Where(item => item.Id == id).FirstOrDefault();
        if (item != null)
        {
            return Results.Ok(item);
        }
        return Results.NotFound();
    }

    [HttpPost]
    [ProducesResponseType<ItemDto>(StatusCodes.Status201Created)]
    public IActionResult Post(CreateItemDto createItemDto)
    {
        var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
        items.Add(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    // PUT /items/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
    {
        var existingItem = items.Where(item => item.Id == id).FirstOrDefault();
        if (existingItem != null)
        {
            var updatedItem = existingItem with
            {
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price
            };

            var index = items.FindIndex(existingItem => existingItem.Id == id);
            items[index] = updatedItem;
            return NoContent();
        }
        return NotFound();
    }

    // DELETE /items/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var index = items.FindIndex(existingItem => existingItem.Id == id);
        items.RemoveAt(index);
        return NoContent();
    }
}

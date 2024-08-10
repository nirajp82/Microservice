using Play.Common;

namespace Play.Inventory.Service.Entities;

public class CatalogItem : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
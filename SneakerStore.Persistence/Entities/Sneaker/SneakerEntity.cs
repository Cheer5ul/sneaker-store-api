namespace SneakerStore.Persistence.Entities.Sneaker;

public class SneakerEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = string.Empty;   

    public ICollection<SneakerSizeEntity> Sizes { get; set; } = [];
}
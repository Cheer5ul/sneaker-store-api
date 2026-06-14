using SneakerStore.Core.Results;

namespace SneakerStore.Core.Models.Sneaker;

public class Sneaker
{
    private Sneaker(Guid id, string name, decimal price, string description, ICollection<SneakerSize> sizes,
        string? imageUrl = null)
    {
        Id = id;
        Name = name;
        Price = price;
        Description = description;
        ImageUrl = imageUrl;
        Sizes = sizes;
    }
    
    public Guid Id { get; }
    public string Name { get; }
    public decimal Price { get; }
    public string Description { get; }
    public string? ImageUrl { get; }
    
    public ICollection<SneakerSize> Sizes { get; }

    public static Result<Sneaker> Create(Guid id, string name, decimal price, string description,
        ICollection<SneakerSize> sizes,
        string? imageUrl = null)
    {
        
    }
}
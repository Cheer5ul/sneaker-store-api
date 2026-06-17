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
        List<Error> errors = [];
        
        if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_NAME_LENGTH)
            errors.Add(SneakerErrors.InvalidName(name));
        if (string.IsNullOrWhiteSpace(description) || description.Length > MAX_DESCRIPTION_LENGTH)
            errors.Add(SneakerErrors.InvalidDescription(description));
        if (price <= 0)
            errors.Add(SneakerErrors.InvalidPrice(price));
        
        if(errors.Count > 0)
            return Result<Sneaker>.Failure(errors);
        
        var sneaker = new Sneaker(
            id, name, price, description, sizes, imageUrl);
        
        return Result<Sneaker>.Success(sneaker);
    }
    
    
}

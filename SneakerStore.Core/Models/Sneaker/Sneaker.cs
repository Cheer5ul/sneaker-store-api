using SneakerStore.Core.Results;
using SneakerStore.Core.Results.Errors;
using SneakerStore.Core.Results.Errors.Sneaker;

namespace SneakerStore.Core.Models.Sneaker;

public class Sneaker
{
    public const int MAX_NAME_LENGTH = 100;
    public const int MAX_DESCRIPTION_LENGTH = 5000;
    
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
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public ICollection<SneakerSize> Sizes { get; private set; }

    public static Result<Sneaker> Create(string name, decimal price, string description,
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
            Guid.NewGuid(), name, price, description, sizes, imageUrl);
        
        return Result<Sneaker>.Success(sneaker);
    }
    
    public static Sneaker Reconstitute(Guid id, string name, decimal price, string description,
        ICollection<SneakerSize> sizes,
        string? imageUrl = null)
    {
        var sneaker = new Sneaker(
            id, name, price, description, sizes, imageUrl);
        
        return sneaker;
    }
    
    
    public Result UpdateName(Guid id, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName) || newName.Length > MAX_NAME_LENGTH)
            return Result.Failure([SneakerErrors.InvalidName(newName)]);
        
        Name = newName;
        
        return Result.Success();
    }
}

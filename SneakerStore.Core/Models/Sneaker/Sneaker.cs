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

    /// <summary>
    /// Creates a new Sneaker object with a unique Id after validating name, price, and description.
    /// </summary>
    /// <param name="name">The Sneaker's name, must be non-empty and at most 100 characters.</param>
    /// <param name="price">The Sneaker's price, must be greater than zero.</param>
    /// <param name="description">The description, must be non-empty and at most 5000 characters.</param>
    /// <param name="sizes">The collection of available sizes.</param>
    /// <param name="imageUrl">The optional image URL.</param>
    /// <returns>A successful result containing the created Sneaker, or a failed result containing validation errors.</returns>
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
    
    /// <summary>
    /// Reconstitutes an existing Sneaker from persisted data without re-validating
    /// (assumed valid since it was previously created with validation and stored).
    /// </summary>
    /// <param name="id">The Sneaker's id.</param>
    /// <param name="name">The Sneaker's name.</param>
    /// <param name="price">The Sneaker's price.</param>
    /// <param name="description">The Sneaker's description.</param>
    /// <param name="sizes">The collection of available sizes.</param>
    /// <param name="imageUrl">The optional image URL.</param>
    /// <returns>A Sneaker object with properties, created with ones given in parameters.</returns>
    public static Sneaker Reconstitute(Guid id, string name, decimal price, string description,
        ICollection<SneakerSize> sizes,
        string? imageUrl = null)
    {
        var sneaker = new Sneaker(
            id, name, price, description, sizes, imageUrl);
        
        return sneaker;
    }
    
    /// <summary>
    /// Updates the Sneaker's name after validating length and content constraints. 
    /// </summary>
    /// <param name="newName">The new name, must be non-empty and at most 100 characters.</param>
    /// <returns>Success if updated, Failure with validation errors otherwise.</returns>
    public Result UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName) || newName.Length > MAX_NAME_LENGTH)
            return Result.Failure([SneakerErrors.InvalidName(newName)]);
        
        Name = newName;
        
        return Result.Success();
    }

    /// <summary>
    /// Updates the Sneaker's price after validating it.
    /// </summary>
    /// <param name="newPrice">The new price, must be greater than zero.</param>
    /// <returns>Success if updated, Failure with validation errors otherwise.</returns>
    public Result UpdatePrice(decimal newPrice)
    {
        if(newPrice < 0)
            return Result.Failure([SneakerErrors.InvalidPrice(Price)]);
        
        Price = newPrice;
        return Result.Success();
    }
    
    /// <summary>
    /// Updates the Sneaker's description after validating length and content constraints. 
    /// </summary>
    /// <param name="newDescription">The new description, must be non-empty and at most 5000 characters</param>
    /// <returns>Success if updated, Failure with validation errors otherwise.</returns>
    public Result UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription) || newDescription.Length > MAX_DESCRIPTION_LENGTH)
            return Result.Failure([SneakerErrors.InvalidDescription(newDescription)]);
        
        Description = newDescription;
        
        return Result.Success();
    }

    /// <summary>
    /// Updates the Sneaker's ImageUrl after validating content constraints. 
    /// </summary>
    /// <param name="newImageUrl">The new ImageUrl, must be non-empty string.</param>
    /// <returns>Success if updated, Failure with validation errors otherwise.</returns>
    public Result UpdateImageUrl(string newImageUrl)
    {
        if(string.IsNullOrWhiteSpace(ImageUrl))
            return Result.Failure([SneakerErrors.InvalidImageUrl(newImageUrl)]);
        
        ImageUrl = newImageUrl;
        
        return Result.Success();
    }
}

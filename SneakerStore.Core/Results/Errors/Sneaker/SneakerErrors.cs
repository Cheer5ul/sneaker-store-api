
namespace SneakerStore.Core.Results.Errors.Sneaker;

public static class SneakerErrors
{
    private static class Codes
    {
        public const string InvalidName = "Sneaker.InvalidName";       
        public const string InvalidDescription = "Sneaker.InvalidDescription";
        public const string InvalidPrice = "Sneaker.InvalidPrice";
        public const string NotFound = "Sneaker.NotFound";
        public const string InvalidImageUrl = "Sneaker.InvalidImageUrl";
    }

    public static Error InvalidName(string name)
        => new Error(Codes.InvalidName, $"Name {name} is invalid.");
    
    public static Error InvalidDescription(string description)
        => new Error(Codes.InvalidDescription, $"Description {description} is invalid.");
    
    public static Error InvalidPrice(decimal price)
        => new Error(Codes.InvalidPrice, $"Price {price} is invalid.");
    public static Error NotFound(Guid id)
        => new Error(Codes.NotFound, $"Sneaker with id {id} is not found.");
    
    public static Error InvalidImageUrl(string imageUrl)
        => new Error(Codes.InvalidImageUrl, $"ImageUrl {imageUrl} is invalid.");
}
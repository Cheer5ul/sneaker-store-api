namespace SneakerStore.Core.Results.Errors.Sneaker;

public static class SneakerSizeErrors
{
    private static class Codes
    {
        public const string InvalidSize = "SneakerSize.InvalidSize"; 
        public const string InvalidRemainedInStockNumber = "SneakerSize.InvalidRemainedInStockNumber";
    }
    
    public static Error InvalidSize(decimal size)
        => new Error(Codes.InvalidSize, $"Size {size} is invalid.");
    
    public static Error InvalidRemainedInStockNumber(int remainedInStock)
        => new Error(Codes.InvalidRemainedInStockNumber, $"Amount of remained items {remainedInStock} is invalid.");
}
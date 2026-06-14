namespace SneakerStore.Core.Models.Sneaker;

public class SneakerSize
{
    public Guid Id { get; }
    public decimal Size { get; }
    public int RemainedInStock { get; }
    
    public Guid SneakerId { get; }
}
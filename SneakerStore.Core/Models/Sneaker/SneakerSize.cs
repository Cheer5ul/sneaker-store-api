using SneakerStore.Core.Results;

namespace SneakerStore.Core.Models.Sneaker;

public class SneakerSize
{
    private SneakerSize(Guid id, decimal size, int remainedInStock, Guid sneakerId)
    {
        Id = id;
        Size = size;
        RemainedInStock = remainedInStock;
        SneakerId = sneakerId;
    }
    public Guid Id { get; }
    public decimal Size { get; }
    public int RemainedInStock { get; }
    
    public Guid SneakerId { get; }

    public static Result<SneakerSize> Create(Guid id, decimal size, int remainedInStock, Guid sneakerId)
    {
        var sneakerSize = new SneakerSize(
            id, size, remainedInStock, sneakerId);
        
        return  Result<SneakerSize>.Success(sneakerSize);
    }
}
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
        List<Error> errors = []; 
        if(size < MINIMAL_SIZE)
            errors.Add(SneakerSizeErrors.InvalidSize(size));
        if(remainedInStock < 0)
            errors.Add(SneakerSizeErrors.InvalidRemainedInStockNumber(remainedInStock));
        
        if (errors.Count > 0)
            return Result<SneakerSize>.Failure(errors);
        
        var sneakerSize = new SneakerSize(
            id, size, remainedInStock, sneakerId);
        
        return  Result<SneakerSize>.Success(sneakerSize);
    }
    
    public static SneakerSize Reconstitute(Guid id, decimal size, int remainedInStock, Guid sneakerId)
    {
        var sneakerSize = new SneakerSize(
            id, size, remainedInStock, sneakerId);
        
        return sneakerSize;
    }
}
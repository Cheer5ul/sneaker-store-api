using Microsoft.Win32.SafeHandles;
using SneakerStore.Core.Results;
using SneakerStore.Core.Results.Errors;
using SneakerStore.Core.Results.Errors.Sneaker;

namespace SneakerStore.Core.Models.Sneaker;

public class SneakerSize
{
    public const int MINIMAL_SIZE = 10;
    private SneakerSize(Guid id, decimal size, int remainedInStock, Guid sneakerId)
    {
        Id = id;
        Size = size;
        RemainedInStock = remainedInStock;
        SneakerId = sneakerId;
    }
    public Guid Id { get; }
    public decimal Size { get; private set; }
    public int RemainedInStock { get; private set; }
    
    public Guid SneakerId { get; }

    /// <summary>
    /// Creates a new SneakerSize object with a unique Id after validating size and amount of remained items.
    /// </summary>
    /// <param name="size">The size, must be greater than 10.</param>
    /// <param name="remainedInStock">The amount of remained sizes in stock, must be greater than zero.</param>
    /// <param name="sneakerId">The id of the Sneaker that owns this size.</param>
    /// <returns>>A successful result containing the created SneakerSize, or a failed result containing validation errors.</returns>
    public static Result<SneakerSize> Create(decimal size, int remainedInStock, Guid sneakerId)
    {
        List<Error> errors = []; 
        if(size < MINIMAL_SIZE)
            errors.Add(SneakerSizeErrors.InvalidSize(size));
        if(remainedInStock < 0)
            errors.Add(SneakerSizeErrors.InvalidRemainedInStockNumber(remainedInStock));
        
        if (errors.Count > 0)
            return Result<SneakerSize>.Failure(errors);
        
        var sneakerSize = new SneakerSize(
            Guid.NewGuid(), size, remainedInStock, sneakerId);
        
        return  Result<SneakerSize>.Success(sneakerSize);
    }
    
    /// <summary>
    /// Reconstitutes an existing Sneaker from persisted data without re-validating
    /// (assumed valid since it was previously created with validation and stored).
    /// </summary>
    /// <param name="id">The Size's id.</param>
    /// <param name="size">The size of the Sneaker.</param>
    /// <param name="remainedInStock">The amount of sizes remained in the stock.</param>
    /// <param name="sneakerId">The id of the Sneaker that owns this size.</param>
    /// <returns>A SneakerSize object with properties, created with ones given in parameters.</returns>
    public static SneakerSize Reconstitute(Guid id, decimal size, int remainedInStock, Guid sneakerId)
    {
        var sneakerSize = new SneakerSize(
            id, size, remainedInStock, sneakerId);
        
        return sneakerSize;
    }

    /// <summary>
    /// Updates the SneakerSize's size after validating it.
    /// </summary>
    /// <param name="newSize">The new size, must be greater than minimal size (10).</param>
    /// <returns>Success if updated, Failure with validation errors otherwise.</returns>
    public Result UpdateSize(decimal newSize)
    {
        if (newSize < MINIMAL_SIZE)
        {
            return Result.Failure([SneakerSizeErrors.InvalidSize(newSize)]);
        }
        
        Size = newSize;
        return Result.Success();
    }

    /// <summary>
    /// Updates amount of the remained sizes after the validation.
    /// </summary>
    /// <param name="newRemainedInStock">The new amount of remained items, must be greater than zero.</param>
    /// <returns>Success if updated, Failure with validation errors otherwise.</returns>
    public Result UpdateRemainedInStock(int newRemainedInStock)
    {
        if (newRemainedInStock < 0)
        {
            return Result.Failure([SneakerSizeErrors.InvalidRemainedInStockNumber(newRemainedInStock)]);
        }
        
        RemainedInStock = newRemainedInStock;
        return Result.Success();
    }
}
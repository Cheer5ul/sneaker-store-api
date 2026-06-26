using Microsoft.EntityFrameworkCore;
using SneakerStore.Core.Interfaces.Repositories.Sneaker;
using SneakerStore.Core.Models.Sneaker;
using SneakerStore.Core.Results;
using SneakerStore.Core.Results.Errors;
using SneakerStore.Core.Results.Errors.Sneaker;
using SneakerStore.Persistence.Entities.Sneaker;

namespace SneakerStore.Persistence.Repositories.Sneaker;

public class SneakerRepository(SneakerStoreDbContext dbContext) : ISneakerRepository
{
    public async Task<List<Core.Models.Sneaker.Sneaker>> GetAll(CancellationToken cancellationToken = default)
    {
        var sneakerEntities = await dbContext.Sneakers
            .AsNoTracking()
            .Include(s => s.Sizes)
            .ToListAsync(cancellationToken);

        var sneakers = sneakerEntities.Select(sneakerEntity =>
            Core.Models.Sneaker.Sneaker.Reconstitute(
                sneakerEntity.Id,
                sneakerEntity.Name,
                sneakerEntity.Price,
                sneakerEntity.Description,
                sneakerEntity.Sizes.Select(sizeEntity => SneakerSize.Reconstitute(
                    sizeEntity.Id,
                    sizeEntity.Size,
                    sizeEntity.RemainedInStock,
                    sizeEntity.SneakerId)).ToList(),
                sneakerEntity.ImageUrl
            )).ToList();

        return sneakers;
    }
    
    public async Task<Core.Models.Sneaker.Sneaker> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var sneakerEntity = await dbContext.Sneakers.Include(sneakerEntity => sneakerEntity.Sizes)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        var sneaker = Core.Models.Sneaker.Sneaker.Reconstitute(
            sneakerEntity!.Id,
            sneakerEntity.Name,
            sneakerEntity.Price,
            sneakerEntity.Description,
            sneakerEntity.Sizes.Select(
                ssEntity => SneakerSize.Reconstitute(
                    ssEntity.Id,
                    ssEntity.Size,
                    ssEntity.RemainedInStock,
                    ssEntity.SneakerId)).ToList(),
            sneakerEntity.ImageUrl);

        return sneaker;
    } 

    public async Task<bool> SneakerExists(Guid id, CancellationToken cancellationToken = default)
    {
        var sneakerEntity = await dbContext.Sneakers
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        
        return sneakerEntity != null;
    }

    public async Task<Guid> Create(Core.Models.Sneaker.Sneaker sneaker,
        CancellationToken cancellationToken = default)
    {
        var sneakerEntity = new SneakerEntity()
        {
            Id = sneaker.Id,
            Name = sneaker.Name,
            Price = sneaker.Price,
            Description = sneaker.Description,
            Sizes = sneaker.Sizes.Select(sizeEntity => new SneakerSizeEntity()
            {
                Id = sizeEntity.Id,
                RemainedInStock = sizeEntity.RemainedInStock,
                Size = sizeEntity.Size,
                SneakerId = sizeEntity.SneakerId
            }).ToList(),
            ImageUrl = sneaker.ImageUrl
        };

        await dbContext.Sneakers.AddAsync(sneakerEntity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return sneakerEntity.Id;
    }

    #region Granular methods for updates
    
    // need to move validation to Application layer!!!!
    public async Task<(Guid id, List<Error> errors)> UpdateName(Guid id, string newName,
        CancellationToken cancellationToken = default)
    {
        // Loading the entities (methods is mapping Sneaker to domain)
        var result = await GetSneakerEntityAndSneaker(id, cancellationToken);
        if(result.IsFailure) return (id, result.Errors);

        // Calling domain method with validation
        var updateResult = result.Value.sneaker.UpdateName(newName);
        if(updateResult.IsFailure) return (id,updateResult.Errors); // the error
        
        // Mapping back and saving
        result.Value.sneakerEntity.Name = result.Value.sneaker.Name;
        await dbContext.SaveChangesAsync(cancellationToken);

        return (id, [Error.None]);
    }

    public async Task<(Guid id, List<Error> errors)> UpdatePrice(Guid id, decimal newPrice,
        CancellationToken cancellationToken = default)
    {
        // Loading the entities (methods is mapping Sneaker to domain)
        var result = await GetSneakerEntityAndSneaker(id, cancellationToken);
        if(result.IsFailure) return (id, result.Errors);
        
        // Calling domain method with validation
        var updateResult = result.Value.sneaker.UpdatePrice(newPrice);
        if(updateResult.IsFailure) return (id, updateResult.Errors); // the error
        
        // Mapping back and saving
        result.Value.sneakerEntity.Price = result.Value.sneaker.Price;
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return (id, [Error.None]);
    }

    public async Task<(Guid id, List<Error> errors)> UpdateDescription(Guid id, string newDescription,
        CancellationToken cancellationToken = default)
    {
        var result = await GetSneakerEntityAndSneaker(id, cancellationToken);
        if(result.IsFailure) return (id, result.Errors);
        
        var updateResult = result.Value.sneaker.UpdateDescription(newDescription);
        if(updateResult.IsFailure) return (id, updateResult.Errors);
        
        result.Value.sneakerEntity.Description = result.Value.sneaker.Description;
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return (id, [Error.None]);
    }

    public async Task<(Guid id, List<Error> errors)> UpdateImageUrl(Guid id, string newImageUrl,
        CancellationToken cancellationToken = default)
    {
        var result = await GetSneakerEntityAndSneaker(id, cancellationToken);
        if(result.IsFailure) return (id, result.Errors);
        
        var updateResult = result.Value.sneaker.UpdateImageUrl(newImageUrl);
        if(updateResult.IsFailure) return (id, updateResult.Errors);
        
        result.Value.sneakerEntity.ImageUrl = result.Value.sneaker.ImageUrl;
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return (id, [Error.None]);
    }

    private async Task<Result<(SneakerEntity sneakerEntity, Core.Models.Sneaker.Sneaker sneaker)>> GetSneakerEntityAndSneaker(
        Guid id, CancellationToken cancellationToken = default)
    {
        var sneakerEntity = await dbContext.Sneakers
            .Include(s => s.Sizes)
            .FirstOrDefaultAsync(sneakerEntity => sneakerEntity.Id == id, cancellationToken);
        
        if (sneakerEntity == null) return Result<(SneakerEntity, Core.Models.Sneaker.Sneaker)>.Failure([SneakerErrors.NotFound(id)]);
        var sneaker = Core.Models.Sneaker.Sneaker.Reconstitute(
            sneakerEntity.Id,
            sneakerEntity.Name,
            sneakerEntity.Price,
            sneakerEntity.Description,
            sneakerEntity.Sizes.Select(sizeEntity => SneakerSize.Reconstitute(
                sizeEntity.Id,
                sizeEntity.Size,
                sizeEntity.RemainedInStock,
                sizeEntity.SneakerId)).ToList(),
            sneakerEntity.ImageUrl);
        
        return Result<(SneakerEntity, Core.Models.Sneaker.Sneaker)>.Success((sneakerEntity, sneaker));
    }
    
    #endregion

    public async Task Delete(Guid sneakerId,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Sneakers
            .Where(sEntity => sEntity.Id == sneakerId)
            .Include(s => s.Sizes)
            .ExecuteDeleteAsync(cancellationToken);
    }
    
    // Methods to interact with SneakerSize entity
    public async Task<List<SneakerSize>> GetAllSizes(Guid sneakerId,
        CancellationToken cancellationToken = default)
    {
        var sneakerSizeEntities = await dbContext.SneakerSizes
            .AsNoTracking()
            .Where(ssEntity => ssEntity.SneakerId == sneakerId)
            .ToListAsync(cancellationToken);

        var sneakerSizes = sneakerSizeEntities
            .Select(ssEntity => SneakerSize.Reconstitute(
                ssEntity.Id,
                ssEntity.Size,
                ssEntity.RemainedInStock,
                ssEntity.SneakerId)).ToList();
        
        return sneakerSizes;
    }

    public async Task<Guid> CreateSize(Guid sneakerId,
        SneakerSize sneakerSize, CancellationToken cancellationToken = default)
    {
        // make sure the item exists in application
        
        var sneakerEntity = await dbContext.Sneakers
            .Where(sEntity => sEntity.Id == sneakerId).Include(sEntity => sEntity.Sizes)
            .FirstAsync(cancellationToken);

        var sneakerSizeEntity = new SneakerSizeEntity()
        {
            Id = sneakerSize.Id,
            Size = sneakerSize.Size,
            RemainedInStock = sneakerSize.RemainedInStock,
            SneakerId = sneakerEntity.Id
        };
        
        sneakerEntity.Sizes.Add(sneakerSizeEntity);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return sneakerSizeEntity.Id;
    }

    // public async Task UpdateSizeSize(Guid sneakerId, Guid sneakerSizeId,
    //     decimal newSize)
    // {
    //  // make sure the item exists in application
    // }

    public async Task DeleteSize(Guid sneakerId,
        Guid sneakerSizeId, CancellationToken cancellationToken = default)
    {
        // make sure the item exists in application
        
        await dbContext.SneakerSizes
            .Where(ssEntity => ssEntity.SneakerId == sneakerId
            && ssEntity.Id == sneakerSizeId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
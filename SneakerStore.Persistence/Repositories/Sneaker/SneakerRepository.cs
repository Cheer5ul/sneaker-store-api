using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SneakerStore.Core.Interfaces.Repositories.Sneaker;
using SneakerStore.Core.Models.Sneaker;
using SneakerStore.Core.Results;
using SneakerStore.Core.Results.Errors;
using SneakerStore.Core.Results.Errors.Sneaker;
using SneakerStore.Persistence.Entities.Sneaker;

namespace SneakerStore.Persistence.Repositories.Sneaker;

public class SneakerRepository(SneakerStoreDbContext dbContext) : ISneakerRepository
{
    
    // TODO: Handle race condition between Application existence check and Repository operations
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
    
    public async Task<Core.Models.Sneaker.Sneaker?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var sneakerEntity = await dbContext.Sneakers.Include(sneakerEntity => sneakerEntity.Sizes)
            // .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        
        if(sneakerEntity == null) return null;

        var sneaker = Core.Models.Sneaker.Sneaker.Reconstitute(
            sneakerEntity.Id,
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
            .AsNoTracking()
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
    
    public async Task UpdateName(Guid id, string newName,
        CancellationToken cancellationToken = default)
    {
        var sneakerEntity = await GetSneakerEntity(id, cancellationToken);
        sneakerEntity.Name = newName;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePrice(Guid id, decimal newPrice,
        CancellationToken cancellationToken = default)
    {
        var sneakerEntity = await GetSneakerEntity(id, cancellationToken);
        sneakerEntity.Price = newPrice;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateDescription(Guid id, string newDescription,
        CancellationToken cancellationToken = default)
    {
        var sneakerEntity = await GetSneakerEntity(id, cancellationToken);
        sneakerEntity.Description = newDescription;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateImageUrl(Guid id, string newImageUrl,
        CancellationToken cancellationToken = default)
    {
        var sneakerEntity = await GetSneakerEntity(id, cancellationToken);
        sneakerEntity.ImageUrl = newImageUrl;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a SneakerEntity be the id. Use when absolutely sure the Sneaker with such an id exists.
    /// </summary>
    /// <param name="id">The id of the Sneaker, Guid.</param>
    /// <param name="cancellationToken">Cancellation token to cancel an asynchronous operation.</param>
    /// <returns>A SneakerEntity with the given id.</returns>
    private async Task<SneakerEntity> GetSneakerEntity(
        Guid id, CancellationToken cancellationToken = default)
    {
        var sneakerEntity = await dbContext.Sneakers
            .Include(s => s.Sizes)
            .FirstAsync(sneakerEntity => sneakerEntity.Id == id, cancellationToken);

        return sneakerEntity!;
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
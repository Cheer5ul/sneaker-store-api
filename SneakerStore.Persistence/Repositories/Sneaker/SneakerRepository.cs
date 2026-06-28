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
    
    public async Task<Core.Models.Sneaker.Sneaker?> GetById(Guid id, bool includeSizes = true,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Sneakers.AsQueryable();

        if (includeSizes)
            query = query.Include(sneakerEntity => sneakerEntity.Sizes);
        
        var sneakerEntity = await query
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        
        if(sneakerEntity == null) return null;

        var sizes = includeSizes
            ? sneakerEntity.Sizes
                .Select(ssEntity => SneakerSize.Reconstitute(
                    ssEntity.Id,
                    ssEntity.Size,
                    ssEntity.RemainedInStock,
                    ssEntity.SneakerId))
                .ToList()
            : [];

        return Core.Models.Sneaker.Sneaker.Reconstitute(
            sneakerEntity.Id,
            sneakerEntity.Name,
            sneakerEntity.Price,
            sneakerEntity.Description,
            sizes,
            sneakerEntity.ImageUrl);
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
    
    public async Task UpdateName(Core.Models.Sneaker.Sneaker sneaker,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Sneakers
            .Where(sEntity => sEntity.Id == sneaker.Id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(entity => entity.Name, sneaker.Name),
                cancellationToken);
    }

    public async Task UpdatePrice(Core.Models.Sneaker.Sneaker sneaker,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Sneakers
            .Where(sEntity => sEntity.Id == sneaker.Id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(entity => entity.Price, sneaker.Price),
                cancellationToken);
    }

    public async Task UpdateDescription(Core.Models.Sneaker.Sneaker sneaker,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Sneakers
            .Where(sEntity => sEntity.Id == sneaker.Id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(entity => entity.Description, sneaker.Description),
                cancellationToken);
    }

    public async Task UpdateImageUrl(Core.Models.Sneaker.Sneaker sneaker,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Sneakers
            .Where(sEntity => sEntity.Id == sneaker.Id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(entity => entity.ImageUrl, sneaker.ImageUrl),
                cancellationToken);
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
    
    #region Methods to interact with SneakerSize entity
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

    public async Task<bool> SneakerSizeExists(Guid sneakerSizeId,
        CancellationToken cancellationToken = default)
    {
        var sneakerSizeEntity = await dbContext.SneakerSizes
            .AsNoTracking()
            .FirstOrDefaultAsync(ssEntity => ssEntity.Id == sneakerSizeId, cancellationToken);
        
        return sneakerSizeEntity != null;
    }

    public async Task<Guid> CreateSize(Guid sneakerId,
        SneakerSize sneakerSize, CancellationToken cancellationToken = default)
    {
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

    public async Task UpdateSneakerSizeSize(Guid sneakerId, Guid sneakerSizeId,
        decimal newSize, CancellationToken cancellationToken = default)
    {
        await dbContext.SneakerSizes
            .Where(ssEntity => ssEntity.SneakerId == sneakerId &&
                               ssEntity.Id == sneakerSizeId)
            .ExecuteUpdateAsync(
                s => s.SetProperty(
                    ssEntity => ssEntity.Size, newSize),
                cancellationToken);
    }

    public async Task UpdateSneakerSizeRemainedInStock(Guid sneakerId, Guid sneakerSizeId,
        int newRemainedInStock, CancellationToken cancellationToken = default)
    {
        await dbContext.SneakerSizes
            .Where(ssEntity => ssEntity.SneakerId == sneakerId &&
                               ssEntity.Id == sneakerSizeId)
            .ExecuteUpdateAsync(
                s => s.SetProperty(
                    ssEntity => ssEntity.RemainedInStock, newRemainedInStock),
                cancellationToken);
    }

    public async Task DeleteSize(Guid sneakerId,
        Guid sneakerSizeId, CancellationToken cancellationToken = default)
    {
        await dbContext.SneakerSizes
            .Where(ssEntity => ssEntity.SneakerId == sneakerId
            && ssEntity.Id == sneakerSizeId)
            .ExecuteDeleteAsync(cancellationToken);
    }
    
    #endregion
}
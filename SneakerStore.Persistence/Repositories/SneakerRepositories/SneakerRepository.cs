using Microsoft.EntityFrameworkCore;
using SneakerStore.Core.Models.Sneaker;
using SneakerStore.Core.Results;
using SneakerStore.Core.Results.Errors;
using SneakerStore.Core.Results.Errors.Sneaker;
using SneakerStore.Persistence.Entities.Sneaker;

namespace SneakerStore.Persistence.Repositories.SneakerRepositories;

public class SneakerRepository
{
    private SneakerStoreDbContext _dbContext;

    public SneakerRepository(SneakerStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Sneaker>> GetAll(CancellationToken cancellationToken = default)
    {
        var sneakerEntities = await _dbContext.Sneakers
            .AsNoTracking()
            .Include(s => s.Sizes)
            .ToListAsync(cancellationToken);

        var sneakers = sneakerEntities.Select(sneakerEntity =>
            Sneaker.Reconstitute(
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

    public async Task<Guid> Create(Sneaker sneaker,
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

        await _dbContext.Sneakers.AddAsync(sneakerEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return sneakerEntity.Id;
    }

    // Granular methods for updates
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
        await _dbContext.SaveChangesAsync(cancellationToken);

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
        await _dbContext.SaveChangesAsync(cancellationToken);
        
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
        await _dbContext.SaveChangesAsync(cancellationToken);
        
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
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return (id, [Error.None]);
    }

    private async Task<Result<(SneakerEntity sneakerEntity, Sneaker sneaker)>> GetSneakerEntityAndSneaker(
        Guid id, CancellationToken cancellationToken = default)
    {
        var sneakerEntity = await _dbContext.Sneakers
            .Include(s => s.Sizes)
            .FirstOrDefaultAsync(sneakerEntity => sneakerEntity.Id == id, cancellationToken);
        
        if (sneakerEntity == null) return Result<(SneakerEntity, Sneaker)>.Failure([SneakerErrors.NotFound(id)]);
        var sneaker = Sneaker.Reconstitute(
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
        
        return Result<(SneakerEntity, Sneaker)>.Success((sneakerEntity, sneaker));
    }
}
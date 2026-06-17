using Microsoft.EntityFrameworkCore;
using SneakerStore.Core.Models.Sneaker;

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
            Sneaker.Create(
                sneakerEntity.Id,
                sneakerEntity.Name,
                sneakerEntity.Price,
                sneakerEntity.Description,
                sneakerEntity.Sizes.Select(sizeEntity => SneakerSize.Create(
                    sizeEntity.Id,
                    sizeEntity.Size,
                    sizeEntity.RemainedInStock,
                    sizeEntity.SneakerId).Value!).ToList(),
                sneakerEntity.ImageUrl
            ).Value!).ToList();

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
    
}
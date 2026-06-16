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
}
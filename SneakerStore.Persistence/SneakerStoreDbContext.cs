using Microsoft.EntityFrameworkCore;
using SneakerStore.Persistence.Entities.Sneaker;
using SneakerStore.Persistence.Entities.User;

namespace SneakerStore.Persistence;

public class SneakerStoreDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public SneakerStoreDbContext(DbContextOptions<SneakerStoreDbContext> options) 
        : base(options)
    {
        
    }
    public DbSet<SneakerEntity> Sneakers { get; set; }
    public DbSet<SneakerSizeEntity> SneakerSizes { get; set; }
    public DbSet<UserEntity> Users { get; set; }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SneakerStore.Persistence.Entities.Sneaker;

namespace SneakerStore.Persistence.Configurations.Sneaker;

public class SneakerConfiguration : IEntityTypeConfiguration<SneakerEntity>
{
    public void Configure(EntityTypeBuilder<SneakerEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.Price)
            .IsRequired();
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ImageUrl);

        builder.HasMany(x => x.Sizes)
            .WithOne(x => x.Sneaker)
            .HasForeignKey(x => x.SneakerId) // implicitly showing foreign key
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}
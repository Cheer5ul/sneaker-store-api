using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SneakerStore.Core.Models.Sneaker;
using SneakerStore.Persistence.Entities.Sneaker;

namespace SneakerStore.Persistence.Configurations.Sneaker;

public class SneakerSizeConfiguration : IEntityTypeConfiguration<SneakerSizeEntity>
{
    public void Configure(EntityTypeBuilder<SneakerSizeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Size)
            .IsRequired()
            .HasPrecision(4,1);
        
        builder.Property(x => x.RemainedInStock)
            .HasDefaultValue(0);
        
        // builder.HasOne(x => x.Sneaker)
        //     .WithMany(x => x.Sizes)
        //     .HasForeignKey(x => x.SneakerId);
    }
}
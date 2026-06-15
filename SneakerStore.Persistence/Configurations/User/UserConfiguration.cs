using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SneakerStore.Persistence.Entities.User;

namespace SneakerStore.Persistence.Configurations.User;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(Core.Models.User.User.MAX_USERNAME_LENGTH);;
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(Core.Models.User.User.MAX_EMAIL_LENGTH);;

        builder.Property(x => x.PasswordHash)
            .IsRequired();
    }
}
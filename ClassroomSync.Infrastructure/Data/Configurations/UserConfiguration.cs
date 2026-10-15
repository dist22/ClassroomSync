using ClassroomSync.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassroomSync.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(x => x.Id);

        builder.Property(u => u.GoogleId)
            .IsRequired();
        
        builder.Property(u => u.Email)
            .IsRequired();
        
        builder.HasIndex(u => u.GoogleId).IsUnique();
        builder.Property(u => u.Email).IsRequired();
    }
}
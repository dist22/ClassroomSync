using ClassroomSync.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassroomSync.Infrastructure.Data.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        
        builder.ToTable("Assignments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.GoogleCourseWorkId).IsRequired();
        builder.Property(a => a.Title).IsRequired();
        
        builder.HasIndex(a => a.GoogleCourseWorkId).IsUnique();
        
        builder.HasOne( a => a.Course)
            .WithMany(b => b.Assignments)
            .HasForeignKey(a => a.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
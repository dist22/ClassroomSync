using ClassroomSync.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassroomSync.Infrastructure.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        
        builder.ToTable("Courses");

        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.GoogleCourseId).IsRequired();
        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.Url).IsRequired();
        
        builder.HasIndex(c => c.GoogleCourseId).IsUnique();

    }
}
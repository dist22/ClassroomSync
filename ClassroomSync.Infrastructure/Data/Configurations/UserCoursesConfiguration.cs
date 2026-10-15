using ClassroomSync.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassroomSync.Infrastructure.Data.Configurations;

public class UserCoursesConfiguration : IEntityTypeConfiguration<UserCourse>
{
    public void Configure(EntityTypeBuilder<UserCourse> builder)
    {
        
        builder.ToTable("UserCourses");
        
        builder.HasKey( c => new {c.CourseId, c.UserId});

        builder.HasOne( c => c.User)
            .WithMany(u => u.UserCourses)
            .HasForeignKey( c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne( c => c.Course)
            .WithMany(c => c.UserCourses)
            .HasForeignKey( c => c.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}
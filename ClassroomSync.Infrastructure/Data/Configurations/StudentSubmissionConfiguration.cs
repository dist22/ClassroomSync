using ClassroomSync.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassroomSync.Infrastructure.Data.Configurations;

public class StudentSubmissionConfiguration : IEntityTypeConfiguration<StudentSubmission>
{
    public void Configure(EntityTypeBuilder<StudentSubmission> builder)
    {
        
        builder.ToTable("StudentSubmissions");
        
        builder.HasKey(s => s.Id);

        builder.Property(s => s.GoogleSubmissionId).IsRequired();
        builder.Property(s => s.State).IsRequired();
        
        builder.HasOne(s => s.Assignment)
            .WithMany(a => a.Submissions)
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(s => s.User)
            .WithMany(u => u.Submissions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
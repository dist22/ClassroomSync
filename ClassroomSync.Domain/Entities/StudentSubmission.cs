namespace ClassroomSync.Domain.Entities;

public class StudentSubmission
{
    public Guid Id { get; init; }
    public string GoogleSubmissionId { get; set; } = string.Empty;
    
    public string State { get; set; } = string.Empty; 
    
    public double? AssignedGrade { get; set; } 
    
    public Guid AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
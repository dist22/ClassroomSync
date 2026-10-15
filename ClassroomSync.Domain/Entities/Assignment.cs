namespace ClassroomSync.Domain.Entities;

public class Assignment
{
    public Guid Id { get; init; }
    public string GoogleCourseWorkId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; } 
    public double? MaxPoints { get; set; } 

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<StudentSubmission> Submissions { get; set; } = new List<StudentSubmission>();
}
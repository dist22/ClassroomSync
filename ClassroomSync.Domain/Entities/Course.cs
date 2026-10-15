namespace ClassroomSync.Domain.Entities;

public class Course
{
    public Guid Id { get; init; }
    public string GoogleCourseId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
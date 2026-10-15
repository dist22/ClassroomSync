namespace ClassroomSync.Domain.Entities;

public class User
{
    public Guid Id { get; init; }
    public string GoogleId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
    public ICollection<StudentSubmission> Submissions { get; set; } = new List<StudentSubmission>();
    
}
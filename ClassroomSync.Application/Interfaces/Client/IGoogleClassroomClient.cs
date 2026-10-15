using ClassroomSync.Application.DTOs;

namespace ClassroomSync.Application.Interfaces.Client;

public interface IGoogleClassroomClient
{
    public Task<IEnumerable<CourseDto>> GetStudentCoursesAsync(string refreshToken, CancellationToken cancellationToken);
    public Task<IEnumerable<AssignmentDto>> GetAssignmentsAsync(string googleCourseId, string refreshToken, CancellationToken cancellationToken);
    public Task<IEnumerable<SubmissionDto>> GetSubmissionsAsync(string googleCourseId, string refreshToken, CancellationToken cancellationToken = default);
}
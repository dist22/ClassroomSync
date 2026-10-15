namespace ClassroomSync.Application.Interfaces.Sevices;

public interface IAssignmentService
{
    public Task SyncAssignmentsForCoursesAsync(List<string> googleCourseIds, string refreshToken, CancellationToken cancellationToken);
}
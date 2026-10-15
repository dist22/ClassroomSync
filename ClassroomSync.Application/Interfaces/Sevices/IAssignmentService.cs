using ClassroomSync.Application.DTOs;

namespace ClassroomSync.Application.Interfaces.Sevices;

public interface IAssignmentService
{
    public Task SyncAssignmentsForCoursesAsync(List<string> googleCourseIds, string refreshToken, CancellationToken cancellationToken);
    public Task<IEnumerable<AssignmentWithStatusDto>> GetAssignmentForCourseAsync(Guid courseId, Guid userId,
        CancellationToken cancellationToken);
}
using ClassroomSync.Application.Interfaces.Jobs;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Application.Interfaces.Sevices;

namespace ClassroomSync.Application.Jobs;

public class ClassroomWorkflowJob(
    IUserRepository userRepository,
    ICourseService courseService,
    IAssignmentService assignmentService,
    ISubmissionService submissionService) : IClassroomWorkflowJob
{
    public async Task SyncCoursesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null || string.IsNullOrEmpty(user.RefreshToken)) return;

        await courseService.SyncUserCoursesAsync(user.Id, user.RefreshToken, cancellationToken);
    }

    public async Task SyncAssignmentsAndSubmissionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithCoursesAsync(userId, cancellationToken);
        if (user == null || string.IsNullOrEmpty(user.RefreshToken)) return;

        var courseIds = user.UserCourses
            .Select(uc => uc.Course.GoogleCourseId)
            .Where(id => !string.IsNullOrEmpty(id))
            .ToList();

        if (courseIds.Any())
        {
            await assignmentService.SyncAssignmentsForCoursesAsync(courseIds, user.RefreshToken, cancellationToken);
            await submissionService.SyncSubmissionsAsync(courseIds, user.Id, user.RefreshToken, cancellationToken);
        }
    }
}
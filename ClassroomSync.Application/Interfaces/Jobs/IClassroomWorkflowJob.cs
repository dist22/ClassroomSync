namespace ClassroomSync.Application.Interfaces.Jobs;

public interface IClassroomWorkflowJob
{
    public Task SyncCoursesAsync(Guid userId, CancellationToken cancellationToken);
    public Task SyncAssignmentsAndSubmissionsAsync(Guid userId, CancellationToken cancellationToken);
}
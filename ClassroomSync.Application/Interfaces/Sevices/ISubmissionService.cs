namespace ClassroomSync.Application.Interfaces.Sevices;

public interface ISubmissionService
{
    public Task SyncSubmissionsAsync(List<string> googleCourseIds, Guid userId, string refreshToken,
        CancellationToken cancellationToken);
}
namespace ClassroomSync.Application.Interfaces.Sevices;

public interface ICourseServiсe
{
    public Task<List<string>> SyncUserCoursesAsync(Guid userId, string refreshToken,
        CancellationToken cancellationToken);
}
using ClassroomSync.Application.DTOs.Responses;

namespace ClassroomSync.Application.Interfaces.Sevices;

public interface ICourseService
{
    public Task<List<string>> SyncUserCoursesAsync(Guid userId, string refreshToken,
        CancellationToken cancellationToken);
    
    public Task<IEnumerable<CourseResponseDto>> GetUserCoursesAsync(Guid userId,
        CancellationToken cancellationToken);
    
}
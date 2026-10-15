using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Interfaces.Repositories;

public interface ICourseRepository
{
    Task<List<Course>> GetByGoogleIdsAsync(IEnumerable<string> googleCourseIds, CancellationToken cancellationToken);
    Task AddAsync(Course course, CancellationToken cancellationToken);
}
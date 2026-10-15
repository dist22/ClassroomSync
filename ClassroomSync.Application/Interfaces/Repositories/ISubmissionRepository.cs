using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Interfaces.Repositories;

public interface ISubmissionRepository
{
    public Task<List<StudentSubmission>> GetByGoogleIdsAsync (IEnumerable<string> googleSubmissionIds, CancellationToken cancellationToken);
    public Task AddRangeAsync (IEnumerable<StudentSubmission> studentSubmissions, CancellationToken cancellationToken);
    public Task UpdateRangeAsync (IEnumerable<StudentSubmission> studentSubmissions, CancellationToken cancellationToken);
}
using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Interfaces.Repositories;

public interface IAssignmentRepository
{
    public Task<List<Assignment>> GetByGoogleIdsAsync(IEnumerable<string> googleAssignmentIds, CancellationToken cancellationToken);
    public Task AddRangeAsync(IEnumerable<Assignment> assignments, CancellationToken cancellationToken);
    public Task<List<Assignment>> GetAssignmentWhitUserSubmissionAsync(Guid courseId, Guid userId, CancellationToken cancellationToken);
}
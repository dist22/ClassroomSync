using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Domain.Entities;
using ClassroomSync.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ClassroomSync.Infrastructure.Repositories;

public class SubmissionsRepository(AppDbContext dbContext) : ISubmissionRepository
{
    private readonly DbSet<StudentSubmission> _submissions = dbContext.StudentSubmissions;

    public async Task<List<StudentSubmission>> GetByGoogleIdsAsync(IEnumerable<string> googleSubmissionIds,
        CancellationToken cancellationToken)
        => await _submissions
            .Where(s => googleSubmissionIds.Contains(s.GoogleSubmissionId)).ToListAsync(cancellationToken);

    public async Task AddRangeAsync(IEnumerable<StudentSubmission> studentSubmissions,
        CancellationToken cancellationToken)
        => await _submissions.AddRangeAsync(studentSubmissions, cancellationToken);

    public Task UpdateRangeAsync(IEnumerable<StudentSubmission> studentSubmissions, CancellationToken cancellationToken)
    {
        _submissions.UpdateRange(studentSubmissions);
        return Task.CompletedTask;
    }
}
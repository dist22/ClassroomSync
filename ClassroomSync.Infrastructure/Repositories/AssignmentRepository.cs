using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Domain.Entities;
using ClassroomSync.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ClassroomSync.Infrastructure.Repositories;

public class AssignmentRepository(AppDbContext dbContext) : IAssignmentRepository
{
    private readonly DbSet<Assignment> _assignments = dbContext.Assignments;

    public async Task<List<Assignment>> GetByGoogleIdsAsync(IEnumerable<string> googleAssignmentIds, CancellationToken cancellationToken)
        => await _assignments
            .Where(a => googleAssignmentIds.Contains(a.GoogleCourseWorkId))
            .ToListAsync(cancellationToken);

    public async Task AddRangeAsync(IEnumerable<Assignment> assignments, CancellationToken cancellationToken) 
        => await _assignments.AddRangeAsync(assignments, cancellationToken);
}
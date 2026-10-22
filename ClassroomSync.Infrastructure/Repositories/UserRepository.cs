using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Domain.Entities;
using ClassroomSync.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ClassroomSync.Infrastructure.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken)
        => await dbContext.Users.FirstOrDefaultAsync(x => x.GoogleId == googleId, cancellationToken);

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<User?> GetUserWithCoursesAsync(Guid id, CancellationToken cancellationToken)
        => await dbContext.Users
            .Include(u => u.UserCourses)
                .ThenInclude(uc => uc.Course)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<List<User>> GetAllUsersWithFullProgressAsync(CancellationToken cancellationToken) 
        => await  dbContext.Users
            .Include(u => u.UserCourses)
                .ThenInclude(u => u.Course)
                .ThenInclude(c => c.Assignments)
            .Include(u => u.Submissions)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
        => await dbContext.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(User user)
    {
        dbContext.Users.Update(user);
        return Task.CompletedTask;
    }
}
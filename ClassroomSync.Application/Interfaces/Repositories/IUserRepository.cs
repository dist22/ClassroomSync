using ClassroomSync.Domain.Entities;

namespace ClassroomSync.Application.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken);
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<User?> GetUserWithCoursesAsync(Guid id, CancellationToken cancellationToken);
    
    public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
    public Task AddAsync(User user, CancellationToken cancellationToken);
    public Task UpdateAsync(User user);
}
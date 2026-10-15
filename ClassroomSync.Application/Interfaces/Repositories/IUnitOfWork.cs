namespace ClassroomSync.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Infrastructure.Data.Context;

namespace ClassroomSync.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken) 
        => await dbContext.SaveChangesAsync(cancellationToken);
}
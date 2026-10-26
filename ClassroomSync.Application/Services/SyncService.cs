using ClassroomSync.Application.Interfaces.Jobs;
using ClassroomSync.Application.Interfaces.Repositories;
using ClassroomSync.Application.Interfaces.Sevices;

namespace ClassroomSync.Application.Services;

public class SyncService(IUserRepository userRepository, IJobDispatcher jobDispatcher) : ISyncService
{
    public async Task TriggerAllUsersSyncAsync(CancellationToken cancellationToken)
    {
        var userIds = await userRepository.GetAllUserIdsAsync(cancellationToken);
        
        foreach (var userId in userIds) jobDispatcher.EnqueueClassroomSync(userId);
        
    }
}
namespace ClassroomSync.Application.Interfaces.Sevices;

public interface ISyncService
{
    public Task TriggerAllUsersSyncAsync(CancellationToken cancellationToken);
}
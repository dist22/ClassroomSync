namespace ClassroomSync.Application.Interfaces.Sevices;

public interface ISyncService
{
    public Task TriggerAllUsersSyncAsync(CancellationToken cancellationToken);
    public void TriggerUserSync(Guid userId, CancellationToken cancellationToken);
    public void TriggerMasterSheetSync();
}
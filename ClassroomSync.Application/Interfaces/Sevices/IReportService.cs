namespace ClassroomSync.Application.Interfaces.Sevices;

public interface IReportService
{
    Task SyncMasterReportAsync(CancellationToken cancellationToken);
}
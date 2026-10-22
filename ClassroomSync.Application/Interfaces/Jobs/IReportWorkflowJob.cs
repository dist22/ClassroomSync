namespace ClassroomSync.Application.Interfaces.Jobs;

public interface IReportWorkflowJob
{
    public Task UpdateGoogleSheetAsync(CancellationToken cancellationToken);
}
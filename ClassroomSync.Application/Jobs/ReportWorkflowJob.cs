using ClassroomSync.Application.Interfaces.Jobs;
using ClassroomSync.Application.Interfaces.Sevices;

namespace ClassroomSync.Application.Jobs;

public class ReportWorkflowJob(IReportService reportService) : IReportWorkflowJob
{
    public async Task UpdateGoogleSheetAsync(CancellationToken cancellationToken)
    {
        await reportService.SyncMasterReportAsync(cancellationToken);
    }
}
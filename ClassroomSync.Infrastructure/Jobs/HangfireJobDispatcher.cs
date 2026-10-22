using ClassroomSync.Application.Interfaces.Jobs;
using Hangfire;

namespace ClassroomSync.Infrastructure.Jobs;

public class HangfireJobDispatcher(IBackgroundJobClient backgroundJobClient) : IJobDispatcher
{
    public void EnqueueClassroomSync(Guid userId)
    {
        var step1Id = backgroundJobClient.Enqueue<IClassroomWorkflowJob>(job =>
            job.SyncCoursesAsync(userId, CancellationToken.None));
        
        var step2Id = backgroundJobClient.ContinueJobWith<IClassroomWorkflowJob>(step1Id, job =>
            job.SyncAssignmentsAndSubmissionsAsync(userId, CancellationToken.None));

        backgroundJobClient.ContinueJobWith<IReportWorkflowJob>(step2Id, job =>
            job.UpdateGoogleSheetAsync(CancellationToken.None));
    }
}
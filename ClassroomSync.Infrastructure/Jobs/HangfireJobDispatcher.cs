using ClassroomSync.Application.Interfaces.Jobs;
using Hangfire;

namespace ClassroomSync.Infrastructure.Jobs;

public class HangfireJobDispatcher(IBackgroundJobClient backgroundJobClient) : IJobDispatcher
{
    public void EnqueueClassroomSync(Guid userId)
    {
        var step1Id = backgroundJobClient.Enqueue<IClassroomWorkflowJob>(job =>
            job.SyncCoursesAsync(userId, CancellationToken.None));
        
        backgroundJobClient.ContinueJobWith<IClassroomWorkflowJob>(step1Id, job =>
            job.SyncAssignmentsAndSubmissionsAsync(userId, CancellationToken.None));
    }

    public void ScheduleSheetUpdate()
    {
        backgroundJobClient.Enqueue<IReportWorkflowJob>(job => job.UpdateGoogleSheetAsync(CancellationToken.None));
    }
}